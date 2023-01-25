using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Relic.DataTypes;
using Relic.Engine;
using static Relic.Editor.GuiFileExplorer;

namespace Relic.Editor
{
    public class GuiFileExplorer : Gui
    {
        // Folder/Files Specific Variables
        public string currentFolder { get { return _currentFolder;  }
            set { _currentFolder = value; DirectoryChanged(); }
        }
        private string _currentFolder;

        // Paths
        private string _mainFolder;
        private string _MainPath;

        // Highlighted
        private enum hoveredType{ None, Folder, File }
        private hoveredType _hoveredType;
        private string _hoveredName;
        private string _hoveredPath;

        // Lists
        private List<FolderInfo> folders = new List<FolderInfo>();
        private List<DataInfo> files = new List<DataInfo>();

        // Events
        public EventHandler OnDirectoryChanged;
        private class DirectoryEventArgs : EventArgs
        {
            public string previousPath = null;
            public string currentPath;
        }
        private void DirectoryChangedEvent(string currentPath)
        {
            EventHandler handler = OnDirectoryChanged;
            var args = new DirectoryEventArgs()
            {
                currentPath = currentPath,
            };

            try
            {
                args.previousPath = new DirectoryInfo(currentPath).Parent.FullName;
            }
            catch{Debug.LogError("[DirectoryChangedEvent] Failed to Read Previous path!");}
            handler?.Invoke(this, args);
        }



        // Gui Specific Variables
        private bool bigIcons;
        private bool selected;

        // Folder Icons
        private Texture _normalFolder;
        private Texture _assetFolder;
        private Texture _artFolder;
        private Texture _scriptsFolder;
        private Texture _pluginsFolder;
        private Texture _musicFolder;
        private Texture _fontsFolder;
        private Texture _tempFolder;
        private Texture _buildFolder;
        private Texture _settingsFolder;
        private Texture _sceneFolder;
        
        // File Icons
        private Texture _normalFile;
        private Texture _tempFile;
        private Texture _sceneFile;
        private Texture _textFile;
        private Texture _artFile;
        private Texture _scriptFile;
        private Texture _musicFile;
        private Texture _settingFile;

        public GuiFileExplorer() : base("File Explorer") { Start(); DirectoryChanged(); }

        public void Start()
        {
            DirectoryInfo info = new DirectoryInfo(Program.projectFolder);

            _mainFolder = @"\" + info.Name;
            _MainPath = Program.projectFolder;
            
            _normalFolder   = Texture.LoadFromResource("Relic.InternalImages.folder.png");
            _assetFolder    = Texture.LoadFromResource("Relic.InternalImages.folder-Assets.png");
            _artFolder      = Texture.LoadFromResource("Relic.InternalImages.folder-Art.png");
            _scriptsFolder  = Texture.LoadFromResource("Relic.InternalImages.folder-Scripts.png");
            _pluginsFolder  = Texture.LoadFromResource("Relic.InternalImages.folder-Plugins.png");
            _musicFolder    = Texture.LoadFromResource("Relic.InternalImages.folder-Music.png");
            _buildFolder    = Texture.LoadFromResource("Relic.InternalImages.folder-Builds.png");
            _settingsFolder = Texture.LoadFromResource("Relic.InternalImages.folder-Settings.png");
            _fontsFolder    = Texture.LoadFromResource("Relic.InternalImages.folder-Fonts.png");
            _tempFolder     = Texture.LoadFromResource("Relic.InternalImages.folder-Temp.png");
            _sceneFolder    = Texture.LoadFromResource("Relic.InternalImages.folder-Scenes.png");

            _normalFile     = Texture.LoadFromResource("Relic.InternalImages.file.png");
            _tempFile       = Texture.LoadFromResource("Relic.InternalImages.file-Temp.png");
            _sceneFile      = Texture.LoadFromResource("Relic.InternalImages.file-Scene.png");
            _textFile       = Texture.LoadFromResource("Relic.InternalImages.file-Text.png");
            _artFile        = Texture.LoadFromResource("Relic.InternalImages.file-Art.png");
            _scriptFile     = Texture.LoadFromResource("Relic.InternalImages.file-Script.png");
            _musicFile      = Texture.LoadFromResource("Relic.InternalImages.file-Music.png");
            _settingFile    = Texture.LoadFromResource("Relic.InternalImages.file-Setting.png");

            if (!Directory.Exists(_MainPath + @"\Assets")) Directory.CreateDirectory(_MainPath + @"\Assets");
        }

        public override void OnGui()
        {
            if (Button("Home")) currentFolder = "";
            SameLine(75);
            if (Button("<", new Vector2(20)))
            {
                if(string.IsNullOrEmpty(currentFolder)) return;

                DirectoryInfo info = new DirectoryInfo(_MainPath + currentFolder);
                string newDir = info.Parent.FullName.Remove(0, _MainPath.Length);
                currentFolder = newDir;
            }

            SameLine(0, 10);
            CheckBox("Big Icons", ref bigIcons);

            Separator();

            Label($"{_mainFolder.Replace(@"\","")}:{currentFolder}");

            BeginChild("FileViewer");
            if (bigIcons) DrawBigIcons();
            else DrawSmallIcons();
            EndChild();

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right) && Window.currentHoveredWindow == windowName)
            {
                ImGui.OpenPopup("ContextMenu");
            }
            

            if (ImGui.BeginPopupContextWindow("ContextMenu"))
            {
                if (_hoveredType == hoveredType.File)
                {
                    Label($"File: {_hoveredName}");
                    if(MenuItem("Delete")){ Debug.LogWarning("Delete not implemented!"); }
                    Separator();
                }
                
                if (_hoveredType == hoveredType.Folder)
                {
                    Label($"Folder: {_hoveredName}");
                    if (MenuItem("Delete")) { Debug.LogWarning("Delete not implemented!"); }
                    Separator();
                }

                if (ImGui.BeginMenu("New"))
                {
                    if (ImGui.MenuItem("Scene"))
                    {
                        if (!File.Exists(@$"{Program.projectFolder}\Assets\Scenes\New Scene.scene"))
                        {
                            SaveManager.CreateDirectory(@"Assets\Scenes");
                            SaveManager.WriteJsonFile<Scene>(new Scene(), @"Assets\Scenes", $"New Scene.scene");
                            ImGui.CloseCurrentPopup();
                        }
                        else { Debug.LogWarning("Please rename \"New Scene.scene\"!"); }
                    }
                    ImGui.EndMenu();
                }

                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    _hoveredType = hoveredType.None;
                    _hoveredPath = "";
                }

                ImGui.EndPopup();
            }


            foreach (var folderInfo in folders)
            {
                if (folderInfo.open) currentFolder += @$"\{folderInfo.folderName}";
            }
        }

        public void DirectoryChanged()
        {
            folders = new List<FolderInfo>();

            foreach (var directory in Directory.GetDirectories(_MainPath + currentFolder))
            {
                DirectoryInfo info = new DirectoryInfo(directory);

                folders.Add(new FolderInfo() { folderName = info.Name});
            }


            files = new List<DataInfo>();

            foreach (var file in Directory.GetFiles(_MainPath + currentFolder))
            {
                FileInfo info = new FileInfo(_MainPath + currentFolder + file);

                files.Add(new DataInfo() { fileName = info.Name });
            }

            DirectoryChangedEvent(_MainPath + currentFolder);
        }

        private void DrawBigIcons()
        {
            int drawnIcons = 0;
            int maxAmountIcons = (int)MathF.Floor(GetContentRegionAvail().X / 100);

            foreach (var directory in Directory.GetDirectories(_MainPath + currentFolder))
            {
                if (drawnIcons == maxAmountIcons) drawnIcons = 0;
                else  SameLine();

                DirectoryInfo info = new DirectoryInfo(directory);

                ImGui.BeginGroup();

                //Button("##Folder", new Vector2(GetContentRegionAvail().X, 20));
                if (Selectable("##Folder", ref selected, ImGuiSelectableFlags.AllowItemOverlap | ImGuiSelectableFlags.AllowDoubleClick,
                        new System.Numerics.Vector2(100)))
                {
                    if (ImGui.IsMouseDoubleClicked(0))
                    {
                        foreach (var folderInfo in folders)
                        {
                            if (folderInfo.folderName == info.Name) folderInfo.open = true;
                        }
                    }
                    
                }
                if (ImGui.IsItemHovered())
                {
                    _hoveredType = hoveredType.Folder;

                    foreach (var folderInfo in folders)
                    {
                        if (folderInfo.folderName == info.Name)
                        {
                            _hoveredPath = _MainPath + currentFolder + @"\" + folderInfo.folderName;
                            _hoveredName = folderInfo.folderName;
                        }

                    }
                }

                SameLine(5);

                ImGui.BeginGroup();
                SelectFolderImage(info.Name, 80);

                string folderName = info.Name;
                if (info.Name.Length > 11)
                {
                    folderName = info.Name.Remove(11, info.Name.Length - 11);
                    folderName += "...";
                }

                Label(folderName);
                ImGui.EndGroup();

                ImGui.EndGroup();
                drawnIcons++;
            }



            foreach (var file in Directory.GetFiles(_MainPath + currentFolder))
            {
                if (drawnIcons == maxAmountIcons) drawnIcons = 0;
                else SameLine();

                FileInfo info = new FileInfo(_MainPath + currentFolder + file);

                ImGui.BeginGroup();
                if (Selectable("##Folder", ref selected, ImGuiSelectableFlags.AllowItemOverlap | ImGuiSelectableFlags.AllowDoubleClick,
                        new System.Numerics.Vector2(100)))
                if (ImGui.IsMouseDoubleClicked(0))
                {
                    foreach (var fileinfo in files)
                    {
                        if (fileinfo.fileName == info.Name) fileinfo.open = true;
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    _hoveredType = hoveredType.File;

                    foreach (var fileInfo in files)
                    {
                        if (fileInfo.fileName == info.Name)
                        {
                            _hoveredPath = _MainPath + currentFolder + @"\" + fileInfo.fileName;
                            _hoveredName = fileInfo.fileName;
                        }
                    }
                }
                SameLine(5);

                ImGui.BeginGroup();
                SelectFileImage(info.Extension, 80);

                string folderName = info.Name.Replace(info.Extension, "");
                if (info.Name.Length > 11)
                {
                    folderName = info.Name.Remove(11, info.Name.Length - 11);
                    folderName += "...";
                }

                Label(folderName);
                ImGui.EndGroup();

                ImGui.EndGroup();
                drawnIcons++;
            }

        }

        private void DrawSmallIcons()
        {
            foreach (var directory in Directory.GetDirectories(_MainPath + currentFolder))
            {
                DirectoryInfo info = new DirectoryInfo(directory);
                
                if (Selectable("##Folder", ref selected,ImGuiSelectableFlags.AllowItemOverlap | ImGuiSelectableFlags.AllowDoubleClick,new System.Numerics.Vector2(GetContentRegionAvail().X, 20))) if (ImGui.IsMouseDoubleClicked(0))
                {
                    foreach (var folderInfo in folders)
                    {
                        if (folderInfo.folderName == info.Name) folderInfo.open = true;
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    _hoveredType = hoveredType.Folder;

                    foreach (var folderInfo in folders)
                    {
                        if (folderInfo.folderName == info.Name)
                        {
                            _hoveredPath = _MainPath + currentFolder + @"\" + folderInfo.folderName;
                            _hoveredName = folderInfo.folderName;
                        }
                    }
                }

                SameLine(15);
                SelectFolderImage(info.Name, 20);

                SameLine(40);
                Label(info.Name);
            }

            foreach (var file in Directory.GetFiles(_MainPath + currentFolder))
            {
                FileInfo info = new FileInfo(_MainPath + currentFolder + @"\" + file);
                
                if (Selectable("##Folder", ref selected, ImGuiSelectableFlags.AllowItemOverlap | ImGuiSelectableFlags.AllowDoubleClick,new System.Numerics.Vector2(GetContentRegionAvail().X, 20))) if (ImGui.IsMouseDoubleClicked(0))
                {
                    foreach (var fileInfo in files)
                    {
                        if (fileInfo.fileName == info.Name) fileInfo.open = true;
                    }
                }
                if (ImGui.IsItemHovered())
                {
                    _hoveredType = hoveredType.File;

                    foreach (var fileInfo in files)
                    {
                        if (fileInfo.fileName == info.Name)
                        {
                            _hoveredPath = _MainPath + currentFolder + @"\" + fileInfo.fileName;
                            _hoveredName = fileInfo.fileName;
                        }
                    }
                }

                SameLine(15);
                SelectFileImage(info.Extension, 20);

                SameLine(40);
                Label(info.Name.Replace(info.Extension, ""));
            }
        }

        private void SelectFolderImage(string name, int size)
        {
            switch (name.ToLower())
            {
                case "assets":
                    DrawIcon(_assetFolder, size);
                    break;
                case "art":
                case "graphics":
                    DrawIcon(_artFolder, size);
                    break;
                case "scripts":
                case "code":
                    DrawIcon(_scriptsFolder, size);
                    break;
                case "music":
                case "sounds":
                    DrawIcon(_musicFolder, size);
                    break;
                case "builds":
                    DrawIcon(_buildFolder, size);
                    break;
                case "fonts":
                    DrawIcon(_fontsFolder, size);
                    break;
                case "temp":
                    DrawIcon(_tempFolder, size);
                    break;
                case "scenes":
                    DrawIcon(_sceneFolder, size);
                    break;
                case "plugins":
                    DrawIcon(_pluginsFolder, size);
                    break;
                case "settings":
                    DrawIcon(_settingsFolder, size);
                    break;
                default:
                    DrawIcon(_normalFolder, size);
                    break;
            }
        }

        private void SelectFileImage(string name, int size)
        {
            switch (name.ToLower())
            {
                case ".jpeg":
                case ".jpg":
                case ".bmp":
                case ".png":
                    DrawIcon(_artFile, size);
                    break;
                case ".cs":
                    DrawIcon(_scriptFile, size);
                    break;
                case ".wav":
                case ".mp3":
                    DrawIcon(_musicFile, size);
                    break;
                case ".txt":
                    DrawIcon(_textFile, size);
                    break;
                case ".project":
                    DrawIcon(_settingFile, size);
                    break;
                case ".scene":
                    DrawIcon(_sceneFile, size);
                    break;
                case ".temp":
                    DrawIcon(_tempFile, size);
                    break;
                default:
                    DrawIcon(_normalFile, size);
                    break;
            }
        }

        public void DrawIcon(Texture texture, int size)
        {
            Image((IntPtr)texture.handle, new System.Numerics.Vector2(size, size), new System.Numerics.Vector2(0, 1), new System.Numerics.Vector2(1, 0));
        }

        public class FolderInfo
        {
            public string folderName;
            public bool open;
        }

        public class DataInfo
        {
            public string fileName;
            public bool open;
        }
    }
}
