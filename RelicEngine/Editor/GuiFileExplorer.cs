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

namespace Relic.Editor
{
    public class GuiFileExplorer : Gui
    {
        private string _mainFolder;
        private string _MainPath;


        public string currentFolder { get { return _currentFolder;  }
            set { _currentFolder = value; DirectoryChanged(); }
        }
        private string _currentFolder;

        public List<FolderInfo> folders = new List<FolderInfo>();
        public List<DataInfo> files = new List<DataInfo>();

        // Icons
        public Texture normalFolder;
        public Texture assetFolder;
        public Texture artFolder;
        public Texture scriptsFolder;
        public Texture pluginsFolder;
        public Texture musicFolder;
        public Texture fontsFolder;

        public Texture normalFile;
        public Texture tempFile;
        public Texture sceneFile;


        private bool bigIcons;

        public GuiFileExplorer() : base("File Explorer") { Start(); DirectoryChanged(); }

        private bool selected;

        public void Start()
        {
            DirectoryInfo info = new DirectoryInfo(Program.projectFolder);

            _mainFolder = "/" + info.Name;
            _MainPath = Program.projectFolder;

            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream textureStream;
            

            textureStream = myAssembly.GetManifestResourceStream("Relic.InternalImages.folder.png");
            normalFolder = Texture.LoadFromBitmap(new Bitmap(textureStream));

            textureStream = myAssembly.GetManifestResourceStream("Relic.InternalImages.folder-Assets.png");
            assetFolder = Texture.LoadFromBitmap(new Bitmap(textureStream));

            textureStream = myAssembly.GetManifestResourceStream("Relic.InternalImages.folder-Art.png");
            artFolder = Texture.LoadFromBitmap(new Bitmap(textureStream));
            
            textureStream = myAssembly.GetManifestResourceStream("Relic.InternalImages.folder-Scripts.png");
            scriptsFolder = Texture.LoadFromBitmap(new Bitmap(textureStream));
            
            textureStream = myAssembly.GetManifestResourceStream("Relic.InternalImages.folder-Plugins.png");
            pluginsFolder = Texture.LoadFromBitmap(new Bitmap(textureStream));

            textureStream = myAssembly.GetManifestResourceStream("Relic.InternalImages.folder-Music.png");
            musicFolder = Texture.LoadFromBitmap(new Bitmap(textureStream));

            textureStream = myAssembly.GetManifestResourceStream("Relic.InternalImages.folder-Fonts.png");
            fontsFolder = Texture.LoadFromBitmap(new Bitmap(textureStream));



            textureStream = myAssembly.GetManifestResourceStream("Relic.InternalImages.file.png");
            normalFile = Texture.LoadFromBitmap(new Bitmap(textureStream));

            textureStream = myAssembly.GetManifestResourceStream("Relic.InternalImages.file-Temp.png");
            tempFile = Texture.LoadFromBitmap(new Bitmap(textureStream));

            textureStream = myAssembly.GetManifestResourceStream("Relic.InternalImages.file-Scene.png");
            sceneFile = Texture.LoadFromBitmap(new Bitmap(textureStream));


            if (!Directory.Exists(_MainPath + "/Assets")) Directory.CreateDirectory(_MainPath + "/Assets");
        }

        public override void OnGui()
        {
            if (Button("Home")) currentFolder = "";

            SameLine(75);
            if (Button("<", new Vector2(20)))
            {
                if(currentFolder == "") return;

                DirectoryInfo info = new DirectoryInfo(_MainPath + currentFolder);
                string newDir = info.Parent.FullName.Remove(0, _MainPath.Length);
                currentFolder = newDir;
            }

            SameLine(0, 10);
            CheckBox("Big Icons", ref bigIcons);

            Separator();

            Label($"{_mainFolder.Replace("/","")}:{currentFolder}");

            BeginChild("FileViewer");
            if(!bigIcons) DrawSmallIcons();
            else DrawBigIcons();
            EndChild();


            foreach (var folderInfo in folders)
            {
                if (folderInfo.open) currentFolder += $"/{folderInfo.folderName}";
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
                if (Selectable("##Folder", ref selected,
                        ImGuiSelectableFlags.AllowItemOverlap | ImGuiSelectableFlags.AllowDoubleClick,
                        new System.Numerics.Vector2(100))) if (ImGui.IsMouseDoubleClicked(0))
                {
                    foreach (var folderInfo in folders)
                    {
                        if (folderInfo.folderName == info.Name) folderInfo.open = true;
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
            
        }

        private void DrawSmallIcons()
        {
            foreach (var directory in Directory.GetDirectories(_MainPath + currentFolder))
            {
                DirectoryInfo info = new DirectoryInfo(directory);

                //Button("##Folder", new Vector2(GetContentRegionAvail().X, 20));
                if (Selectable("##Folder", ref selected,
                        ImGuiSelectableFlags.AllowItemOverlap | ImGuiSelectableFlags.AllowDoubleClick,
                        new System.Numerics.Vector2(GetContentRegionAvail().X, 20))) if (ImGui.IsMouseDoubleClicked(0))
                {
                    foreach (var folderInfo in folders)
                    {
                        if (folderInfo.folderName == info.Name) folderInfo.open = true;
                    }
                }
                SameLine(15);

                SelectFolderImage(info.Name, 20);

                SameLine(40);
                Label(info.Name);
            }

            foreach (var file in Directory.GetFiles(_MainPath + currentFolder))
            {
                FileInfo info = new FileInfo(_MainPath + currentFolder + "/" + file);

                //Button("##Folder", new Vector2(GetContentRegionAvail().X, 20));
                if (Selectable("##Folder", ref selected,
                        ImGuiSelectableFlags.AllowItemOverlap | ImGuiSelectableFlags.AllowDoubleClick,
                        new System.Numerics.Vector2(GetContentRegionAvail().X, 20))) if (ImGui.IsMouseDoubleClicked(0))
                {
                    foreach (var fileInfo in files)
                    {
                        if (fileInfo.fileName == info.Name) fileInfo.open = true;
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
                    DrawIcon(assetFolder, size);
                    break;
                case "art":
                case "graphics":
                    DrawIcon(artFolder, size);
                    break;
                case "scripts":
                case "code":
                    DrawIcon(scriptsFolder, size);
                    break;
                case "music":
                case "sounds":
                    DrawIcon(musicFolder, size);
                    break;
                case "fonts":
                    DrawIcon(fontsFolder, size);
                    break;
                case "plugins":
                    DrawIcon(pluginsFolder, size);
                    break;
                default:
                    DrawIcon(normalFolder, size);
                    break;
            }
        }

        private void SelectFileImage(string name, int size)
        {
            switch (name.ToLower())
            {
                case ".scene":
                    DrawIcon(sceneFile, size);
                    break;
                case ".temp":
                    DrawIcon(tempFile, size);
                    break;
                default:
                    DrawIcon(normalFile, size);
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
