using GH_IO;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using System;
using System.IO;

namespace Hive.GUI
{
    class GH_ClusterFile : GH_ISerializable
    {
        private string m_path;
        private Guid m_hash;
        private GH_Synchronisation m_sync;
        private GH_FileWatcher m_watcher;

        public GH_ClusterFile()
        {
            m_path = (string)null;
            m_hash = Guid.Empty;
            m_sync = GH_Synchronisation.Unset;
        }

        public string Path
        {
            get
            {
                return m_path;
            }
            set
            {
                if (m_path == null)
                {
                    if (value == null)
                    {
                        m_sync = GH_Synchronisation.NotReferenced;
                        m_hash = Guid.Empty;
                        this.DestroyWatcher();
                    }
                    else
                    {
                        m_path = value;
                        m_hash = Guid.Empty;
                        m_sync = GH_Synchronisation.Unset;
                        this.CreateWatcher();
                    }
                }
                else if (value == null)
                {
                    m_path = (string)null;
                    m_hash = Guid.Empty;
                    m_sync = GH_Synchronisation.NotReferenced;
                    this.DestroyWatcher();
                }
                else
                {
                    m_path = value;
                    m_hash = Guid.Empty;
                    m_sync = GH_Synchronisation.Unset;
                    this.CreateWatcher();
                }
            }
        }

        public bool IsPath
        {
            get
            {
                return !string.IsNullOrEmpty(this.Path);
            }
        }

        public GH_ClusterFileType PathType
        {
            get
            {
                GH_ClusterFileType ghClusterFileType;
                if (!this.IsPath)
                {
                    ghClusterFileType = GH_ClusterFileType.None;
                }
                else
                {
                    try
                    {
                        string upperInvariant = System.IO.Path.GetExtension(this.Path).ToUpperInvariant();
                        ghClusterFileType = Operators.CompareString(upperInvariant, ".GH", false) == 0 ? GH_ClusterFileType.Gh : (Operators.CompareString(upperInvariant, ".GHX", false) == 0 ? GH_ClusterFileType.Ghx : (Operators.CompareString(upperInvariant, ".GHCLUSTER", false) == 0 ? GH_ClusterFileType.GhCluster : GH_ClusterFileType.Unknown));
                        goto label_5;
                    }
                    catch (Exception ex)
                    {
                        //ProjectData.SetProjectError(ex);
                        //ProjectData.ClearProjectError();
                    }
                    ghClusterFileType = GH_ClusterFileType.Unknown;
                }
            label_5:
                return ghClusterFileType;
            }
        }

        public Guid Hash
        {
            get
            {
                return this.m_hash;
            }
        }

        public GH_Synchronisation Synchronization
        {
            get
            {
                GH_Synchronisation sync;
                if (this.m_sync != GH_Synchronisation.Unset)
                    sync = this.m_sync;
                else if (!this.IsPath)
                {
                    this.m_sync = GH_Synchronisation.NotReferenced;
                    sync = this.m_sync;
                }
                else
                {
                    try
                    {
                        if (!File.Exists(this.Path))
                        {
                            this.m_sync = GH_Synchronisation.MissingReference;
                            sync = this.m_sync;
                            goto label_13;
                        }
                        else if (this.Hash == Guid.Empty)
                        {
                            this.m_sync = GH_Synchronisation.OutOfDate;
                            sync = this.m_sync;
                            goto label_13;
                        }
                        else if (this.Hash == GH_Convert.FileToHash(this.Path))
                        {
                            this.m_sync = GH_Synchronisation.UpToDate;
                            sync = this.m_sync;
                            goto label_13;
                        }
                        else
                        {
                            this.m_sync = GH_Synchronisation.OutOfDate;
                            sync = this.m_sync;
                            goto label_13;
                        }
                    }
                    catch (Exception ex)
                    {
                        //ProjectData.SetProjectError(ex);
                        this.m_sync = GH_Synchronisation.MissingReference;
                        //ProjectData.ClearProjectError();
                    }
                    sync = this.m_sync;
                }
            label_13:
                return sync;
            }
        }

        public void UpdateToCurrentFile()
        {
            if (!this.IsPath)
            {
                this.m_sync = GH_Synchronisation.NotReferenced;
                this.m_hash = Guid.Empty;
            }
            else
            {
                try
                {
                    if (File.Exists(this.Path))
                    {
                        this.m_sync = GH_Synchronisation.UpToDate;
                        this.m_hash = GH_Convert.FileToHash(this.Path);
                    }
                    else
                    {
                        this.m_sync = GH_Synchronisation.MissingReference;
                        this.m_hash = Guid.Empty;
                    }
                }
                catch (Exception ex)
                {
                    //ProjectData.SetProjectError(ex);
                    this.m_sync = GH_Synchronisation.Unset;
                    this.m_hash = Guid.Empty;
                    //ProjectData.ClearProjectError();
                }
            }
        }

        private void CreateWatcher()
        {
            this.DestroyWatcher();
            if (!this.IsPath)
                return;
            try
            {
                this.m_watcher = GH_FileWatcher.CreateFileWatcher(this.Path, GH_FileWatcherEvents.All, new GH_FileWatcher.FileChangedSimple(this.WatcherFileChanged));
            }
            catch (Exception ex)
            {
                //ProjectData.SetProjectError(ex);
                this.m_sync = GH_Synchronisation.MissingReference;
                //ProjectData.ClearProjectError();
            }
        }

        private void DestroyWatcher()
        {
            if (this.m_watcher == null)
                return;
            this.m_watcher.Dispose();
            this.m_watcher = (GH_FileWatcher)null;
        }

        private void WatcherFileChanged(string filename)
        {
            if (!this.IsPath)
            {
                this.DestroyWatcher();
            }
            else
            {
                try
                {
                    if (File.Exists(this.Path))
                    {
                        if (!(this.m_hash != GH_Convert.FileToHash(this.Path)))
                            return;
                        this.m_sync = GH_Synchronisation.OutOfDate;
                        // ISSUE: reference to a compiler-generated field
                        GH_ClusterFile.FileChangedEventHandler fileChangedEvent = this.FileChangedEvent;
                        if (fileChangedEvent == null)
                            return;
                        fileChangedEvent();
                    }
                    else
                    {
                        this.m_sync = GH_Synchronisation.MissingReference;
                        // ISSUE: reference to a compiler-generated field
                        GH_ClusterFile.FileChangedEventHandler fileChangedEvent = this.FileChangedEvent;
                        if (fileChangedEvent == null)
                            return;
                        fileChangedEvent();
                    }
                }
                catch (Exception ex)
                {
                    //ProjectData.SetProjectError(ex);
                    this.m_sync = GH_Synchronisation.Unset;
                    // ISSUE: reference to a compiler-generated field
                    GH_ClusterFile.FileChangedEventHandler fileChangedEvent = this.FileChangedEvent;
                    if (fileChangedEvent != null)
                        fileChangedEvent();
                    //ProjectData.ClearProjectError();
                }
            }
        }

        public event GH_ClusterFile.FileChangedEventHandler FileChanged;

        public void Clear()
        {
            this.DestroyWatcher();
            this.m_path = (string)null;
            this.m_hash = Guid.Empty;
            this.m_sync = GH_Synchronisation.Unset;
        }

        public bool Write(GH_IWriter writer)
        {
            if (this.IsPath)
            {
                writer.SetString("Path", this.Path);
                writer.SetPath("FilePath", this.Path, writer.ArchiveLocation);
            }
            if (this.Hash != Guid.Empty)
                writer.SetGuid("Hash", this.Hash);
            return true;
        }

        public bool Read(GH_IReader reader)
        {
            this.Clear();
            if (reader.ItemExists("FilePath"))
            {
                string[] path1 = reader.GetPath("FilePath", reader.ArchiveLocation);
                int index = 0;
                while (index < path1.Length)
                {
                    string path2 = path1[index];
                    this.m_path = path2;
                    try
                    {
                        if (File.Exists(path2))
                            break;
                    }
                    catch (Exception ex)
                    {
                        //ProjectData.SetProjectError(ex);
                        //ProjectData.ClearProjectError();
                    }
                    checked { ++index; }
                }
            }
            else
                reader.TryGetString("Path", ref this.m_path);

            reader.TryGetGuid("Hash", ref this.m_hash);
            this.CreateWatcher();
            return true;
        }

        public delegate void FileChangedEventHandler();
    }
}
