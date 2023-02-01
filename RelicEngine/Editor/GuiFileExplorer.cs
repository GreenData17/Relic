using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ImGuiNET;
using Relic.DataTypes;
using Relic.Engine;
using Debug = Relic.Engine.Debug;

namespace Relic.Editor
{
    public class GuiFileExplorer : Gui
    {
        // Folder/Files Specific Variables
        public string currentFolder { get => _currentFolder;
            set { _currentFolder = value; DirectoryChanged(); }
        }
        private string _currentFolder;

        // Paths
        private string _mainFolder;
        private string _mainPath;

        // Highlighted
        private enum HoveredType{ Folder, File }
        private HoveredType _hoveredType;
        private string _hoveredName;

        // Lists
        private List<FolderInfo> _folders = new();
        private List<DataInfo> _files = new();

        // Events
        public EventHandler OnDirectoryChanged;
        private class DirectoryEventArgs : EventArgs
        {
            // ReSharper disable once NotAccessedField.Local
            public string previousPath;
            // ReSharper disable once NotAccessedField.Local
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
                args.previousPath = new DirectoryInfo(currentPath).Parent!.FullName;
            }
            catch{Debug.LogError("[DirectoryChangedEvent] Failed to Read Previous path!");}
            handler?.Invoke(this, args);
        }



        // Gui Specific Variables
        private bool _bigIcons;
        private bool _selected;

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
            _mainPath = Program.projectFolder;
            
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

            if (!Directory.Exists(_mainPath + @"\Assets")) Directory.CreateDirectory(_mainPath + @"\Assets");
        }

        public override void OnGui()
        {
            if (Button("Home")) currentFolder = "";
            SameLine(75);
            if (Button("<", new Vector2(20)))
            {
                if(string.IsNullOrEmpty(currentFolder)) return;

                if (Directory.Exists(_mainPath + currentFolder))
                {
                    DirectoryInfo info = new DirectoryInfo(_mainPath + currentFolder);
                    string newDir = info.Parent!.FullName.Remove(0, _mainPath.Length);
                    currentFolder = newDir;
                }
            }

            SameLine(0, 10);
            CheckBox("Big Icons", ref _bigIcons);

            Separator();

            Label($"{_mainFolder.Replace(@"\","")}:{currentFolder}");

            BeginChild("FileViewer");
            if (_bigIcons) DrawBigIcons();
            else DrawSmallIcons();
            EndChild();

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right) && Window.currentHoveredWindow == windowName)
            {
                ImGui.OpenPopup("ContextMenu");
            }
            
            // == Menu Item's you can only select if right clicking a File or Folder ==

            if (ImGui.BeginPopupContextWindow("ContextMenu"))
            {
                if (_hoveredType == HoveredType.File || _hoveredType == HoveredType.Folder)
                {
                    string displayType = _hoveredType == HoveredType.File ? "File" : "Folder";
                    Label($"{displayType}: {_hoveredName}");


                    if (MenuItem("Delete"))
                    {
                        if(_hoveredType == HoveredType.File) 
                            File.Delete($@"{Program.projectFolder}{_currentFolder}\{_hoveredName}");
                        else
                            Directory.Delete($@"{Program.projectFolder}{_currentFolder}\{_hoveredName}");
                    }



                    Separator();
                }

                // == Menu Item's always available ==

                if (MenuItem("Reveal in Explorer"))
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        Arguments = $@"{Program.projectFolder}{_currentFolder}",
                        FileName = "explorer.exe",
                    };

                    Process.Start(startInfo);
                }

                // == "New" Menu ==

                if (ImGui.BeginMenu("New"))
                {
                    if (ImGui.MenuItem("Scene"))
                    {
                        if (!File.Exists(@$"{Program.projectFolder}\Assets\Scenes\New Scene.scene"))
                        {
                            SaveManager.CreateDirectory(@"Assets\Scenes");
                            SaveManager.WriteJsonFile<Scene>(new Scene(), @"Assets\Scenes", "New Scene.scene");
                            ImGui.CloseCurrentPopup();
                        }
                        else { Debug.LogWarning("Please rename \"New Scene.scene\"!"); }
                    }
                    if (ImGui.MenuItem("Textfile"))
                    {
                        if (!File.Exists($@"{Program.projectFolder}{_currentFolder}\new textfile.txt"))
                        {
                            SaveManager.WriteFile("", $@"{_currentFolder}", "new textfile.txt");
                        }
                    }

                    ImGui.EndMenu();
                }

                ImGui.EndPopup();
            }


            foreach (var folderInfo in _folders)
            {
                if (folderInfo.open) currentFolder += @$"\{folderInfo.folderName}";
            }
        }

        public void DirectoryChanged()
        {
            _folders = new List<FolderInfo>();

            foreach (var directory in Directory.GetDirectories(_mainPath + currentFolder))
            {
                DirectoryInfo info = new DirectoryInfo(directory);

                _folders.Add(new FolderInfo() { folderName = info.Name});
            }


            _files = new List<DataInfo>();

            foreach (var file in Directory.GetFiles(_mainPath + currentFolder))
            {
                FileInfo info = new FileInfo(_mainPath + currentFolder + file);

                _files.Add(new DataInfo() { fileName = info.Name });
            }

            DirectoryChangedEvent(_mainPath + currentFolder);
        }


        private void DrawSelectable(System.Numerics.Vector2 size, DirectoryInfo info)
        {
            if (Selectable("##Folder", ref _selected, ImGuiSelectableFlags.AllowItemOverlap | ImGuiSelectableFlags.AllowDoubleClick, size))
            {
                if (ImGui.IsMouseDoubleClicked(0))
                {
                    foreach (var folderInfo in _folders)
                    {
                        if (folderInfo.folderName == info.Name) folderInfo.open = true;
                    }
                }
            }
        }
        private void DrawSelectable(System.Numerics.Vector2 size, FileInfo info)
        {
            if (Selectable("##File", ref _selected, ImGuiSelectableFlags.AllowItemOverlap | ImGuiSelectableFlags.AllowDoubleClick, size))
            {
                if (ImGui.IsMouseDoubleClicked(0))
                {
                    foreach (var fileInfo in _files)
                    {
                        if (fileInfo.fileName == info.Name) fileInfo.open = true;
                    }

                    if (info.Extension == ".scene")
                    {
                        Window.instance.currentScene.LoadGameobjects("Assets\\Scenes\\"+info.Name);
                    }
                }
            }
        }

        private void IsItemHovered(DirectoryInfo info)
        {
            if (ImGui.IsItemHovered())
            {
                _hoveredType = HoveredType.Folder;

                foreach (var folderInfo in _folders)
                {
                    if (folderInfo.folderName == info.Name)
                    {
                        //_hoveredPath = _MainPath + currentFolder + @"\" + folderInfo.folderName;
                        _hoveredName = folderInfo.folderName;
                    }
                }
            }
        }
        private void IsItemHovered(FileInfo info)
        {
            if (ImGui.IsItemHovered())
            {
                _hoveredType = HoveredType.File;

                foreach (var fileInfo in _files)
                {
                    if (fileInfo.fileName == info.Name)
                    {
                        //_hoveredPath = _MainPath + currentFolder + @"\" + fileInfo.fileName;
                        _hoveredName = fileInfo.fileName;
                    }
                }
            }
        }

        private static string MaxStringLength(int length, DirectoryInfo info)
        {
            string folderName = info.Name;
            if (info.Name.Length > length)
            {
                folderName = info.Name.Remove(length, info.Name.Length - length);
                folderName += "...";
            }
            return folderName;
        }
        private static string MaxStringLength(int length, FileInfo info)
        {
            string fileName = info.Name;
            if (info.Name.Length > length)
            {
                fileName = info.Name.Remove(length, info.Name.Length - length);
                fileName += "...";
            }
            return fileName;
        }

        private bool IgnoreFiles(FileInfo info)
        {
            if (info.Extension == ".project")
            {
                return true;
            }

            return false;
        }


        private void DrawBigIcons()
        {
            int drawnIcons = 0;
            int maxAmountIcons = (int)MathF.Floor(GetContentRegionAvail().X / 100);

            foreach (var directory in Directory.GetDirectories(_mainPath + currentFolder))
            {
                if (drawnIcons == maxAmountIcons) drawnIcons = 0;
                else  SameLine();

                DirectoryInfo info = new DirectoryInfo(directory);

                ImGui.BeginGroup();

                    DrawSelectable(new System.Numerics.Vector2(100), info);
                    IsItemHovered(info);
                    SameLine(5);
                    ImGui.BeginGroup();

                        SelectFolderImage(info.Name, 80);
                        Label(MaxStringLength(11, info));

                    ImGui.EndGroup();

                ImGui.EndGroup();
                drawnIcons++;
            }



            foreach (var file in Directory.GetFiles(_mainPath + currentFolder))
            {
                if (drawnIcons == maxAmountIcons) drawnIcons = 0;
                else SameLine();
                
                FileInfo info = new FileInfo(_mainPath + currentFolder + file);

                if (IgnoreFiles(info)) continue;

                ImGui.BeginGroup();

                    DrawSelectable(new System.Numerics.Vector2(100), info);
                    IsItemHovered(info);
                    SameLine(5);
                    ImGui.BeginGroup();

                        SelectFileImage(info.Extension, 80);
                        Label(MaxStringLength(11, info));

                    ImGui.EndGroup();

                ImGui.EndGroup();
                drawnIcons++;
            }

        }

        private void DrawSmallIcons()
        {
            foreach (var directory in Directory.GetDirectories(_mainPath + currentFolder))
            {
                DirectoryInfo info = new DirectoryInfo(directory);
                
                DrawSelectable(new System.Numerics.Vector2(GetContentRegionAvail().X, 20), info);
                IsItemHovered(info);

                SameLine(15);
                SelectFolderImage(info.Name, 20);

                SameLine(40);
                Label(info.Name);
            }

            foreach (var file in Directory.GetFiles(_mainPath + currentFolder))
            {
                FileInfo info = new FileInfo(_mainPath + currentFolder + @"\" + file);

                if (IgnoreFiles(info)) continue;

                DrawSelectable(new System.Numerics.Vector2(GetContentRegionAvail().X, 20), info);
                IsItemHovered(info);

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
