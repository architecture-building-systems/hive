using GH_IO;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Undo;
using Grasshopper.Kernel.Undo.Actions;
using Microsoft.VisualBasic.CompilerServices;
using Rhino;
using Rhino.Display;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Hive.GUI
{
    public class GH_Cluster : GH_Component, IGH_VariableParameterComponent, IGH_InstanceGuidDependent, IGH_DocumentOwner
    {
        private byte[] m_password;
        private readonly GH_ClusterFile m_file;
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal GH_Document m_internalDocument;
        private IGH_Author m_author;
        private GH_Cluster.HookParamMap m_mapping;
        private bool m_previewContent;

        public GH_Cluster()
          : base("Cluster",
                 "Cluster",
                 "Cluster testing",
                 "[hive]",
                 "GUI")
        {
            this.m_file = new GH_ClusterFile();
            this.m_mapping = new GH_Cluster.HookParamMap();
            this.m_previewContent = false;
            this.m_file.FileChanged += new GH_ClusterFile.FileChangedEventHandler(this.ReferenceFileChanged);
            this.m_author = CentralSettings.AuthorDetails;
        }

        private void ReferenceFileChanged()
        {
            Instances.InvalidateCanvas();
        }

        public override void AddedToDocument(GH_Document document)
        {
            base.AddedToDocument(document);
            this.m_file.FileChanged -= new GH_ClusterFile.FileChangedEventHandler(this.ReferenceFileChanged);
            this.m_file.FileChanged += new GH_ClusterFile.FileChangedEventHandler(this.ReferenceFileChanged);
        }

        public override void RemovedFromDocument(GH_Document document)
        {
            base.RemovedFromDocument(document);
            this.m_file.FileChanged -= new GH_ClusterFile.FileChangedEventHandler(this.ReferenceFileChanged);
        }

        public override void DocumentContextChanged(GH_Document document, GH_DocumentContext context)
        {
            base.DocumentContextChanged(document, context);
            this.m_file.FileChanged -= new GH_ClusterFile.FileChangedEventHandler(this.ReferenceFileChanged);
            switch (context)
            {
                case GH_DocumentContext.Open:
                case GH_DocumentContext.Loaded:
                    this.m_file.FileChanged += new GH_ClusterFile.FileChangedEventHandler(this.ReferenceFileChanged);
                    break;
            }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        public override void CreateAttributes()
        {
            this.m_attributes = (IGH_Attributes)new GH_ClusterAttributes(this);
        }

        protected override Bitmap Icon
        {
            get
            {
                return Icons.compGroups;
            }
        }

        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.hidden;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("06850b51-3c70-41f4-86e7-9641114ac549"); }
        }

        public string FilePath
        {
            get
            {
                return this.m_file.Path;
            }
        }

        public GH_Synchronisation Synchronisation
        {
            get
            {
                return this.m_file.Synchronization;
            }
        }

        internal IGH_Author Author
        {
            get
            {
                return this.m_author;
            }
            set
            {
                if (value == null)
                    value = (IGH_Author)new GH_Author();
                this.m_author = value;
            }
        }

        public GH_Document Document(string password)
        {
            return this.IsPasswordValid(password) ? this.m_internalDocument : (GH_Document)null;
        }

        public Guid DocumentId
        {
            get
            {
                return this.m_internalDocument != null ? this.m_internalDocument.DocumentID : Guid.Empty;
            }
            set
            {
                if (this.m_internalDocument == null)
                    return;
                //this.m_internalDocument.SetDocumentID(value);
            }
        }

        public override bool Locked
        {
            get
            {
                return base.Locked;
            }
            set
            {
                if (this.Locked == value)
                    return;
                base.Locked = value;
                if (this.m_internalDocument == null)
                    return;
                if (value)
                    this.m_internalDocument.Context = GH_DocumentContext.Lock;
                else
                    this.m_internalDocument.Context = GH_DocumentContext.Unlock;
            }
        }

        public void CreateFromDocument(GH_Document document)
        {
            this.UpdateDocument(document);
        }

        public void CreateFromDocument(GH_Document document, System.Drawing.Point location)
        {
            this.CreateFromDocument(document);
            if (this.m_attributes == null)
                this.CreateAttributes();
            this.Attributes.ExpireLayout();
            this.Attributes.PerformLayout();
            RectangleF bounds = this.Attributes.Bounds;
            float width = (float)location.X - (float)(0.5 * ((double)bounds.Left + (double)bounds.Right));
            float height = (float)location.Y - (float)(0.5 * ((double)bounds.Top + (double)bounds.Bottom));
            IGH_Attributes attributes;
            PointF pointF = (attributes = this.Attributes).Pivot + new SizeF(width, height);
            attributes.Pivot = pointF;
            this.Attributes.ExpireLayout();
        }

        public void CreateFromSelection(GH_Document sourceDocument, bool deleteOriginalObjects, bool reconnectSources, bool reconnectRecipients)
        {
            if (sourceDocument == null)
                throw new ArgumentNullException(nameof(sourceDocument));
            if (sourceDocument.SelectedCount == 0)
                throw new InvalidOperationException("Document does not have any selected objects");
            if (this.OnPingDocument() != null)
                throw new InvalidOperationException("GH_Cluster.CreateFromSelection can only be called on a new cluster");
            if (this.m_internalDocument != null)
            {
                if (this.m_internalDocument == sourceDocument)
                    throw new InvalidOperationException("sourceDocument cannot be the same as the cluster document");
                this.m_internalDocument.Context = GH_DocumentContext.Close;
                this.m_internalDocument.Dispose();
                this.m_internalDocument = (GH_Document)null;
            }

            GH_UndoRecord record = new GH_UndoRecord("Create Cluster");
            GH_DocumentIO ghDocumentIo = new GH_DocumentIO(sourceDocument);
            ghDocumentIo.Copy(GH_ClipboardType.Local, true);
            ghDocumentIo.Paste(GH_ClipboardType.Local);
            GH_Document document = ghDocumentIo.Document;

            if (document == null)
            {
                Tracing.Assert(new Guid("{E15C050C-6946-4056-ADEC-D3E80DB32459}"), "Document selection duplication failed");
            }
            else
            {
                document.DestroyProxySources();
                RectangleF rectangleF = document.BoundingBox(false);
                PointF @in = new PointF(rectangleF.X + 0.5f * rectangleF.Width, rectangleF.Y + 0.5f * rectangleF.Height);
                GH_Cluster.HookTargetMap hookTargetMap = new GH_Cluster.HookTargetMap();
                List<IGH_Attributes> ghAttributesList = new List<IGH_Attributes>((IEnumerable<IGH_Attributes>)document.Attributes);
                List<IGH_Attributes>.Enumerator enumerator1;
                try
                {
                    enumerator1 = ghAttributesList.GetEnumerator();
                    while (enumerator1.MoveNext())
                    {
                        IGH_Param docObject = enumerator1.Current.DocObject as IGH_Param;
                        if (docObject != null)
                        {
                            IGH_Param parameter = sourceDocument.FindParameter(docObject.InstanceGuid);
                            if (parameter != null)
                            {
                                if (docObject.SourceCount != parameter.SourceCount)
                                {
                                    GH_ClusterInputHook clusterInputHook = new GH_ClusterInputHook(docObject);
                                    IEnumerator<IGH_Param> enumerator2;
                                    try
                                    {
                                        enumerator2 = parameter.Sources.GetEnumerator();
                                        while (enumerator2.MoveNext())
                                        {
                                            IGH_Param current = enumerator2.Current;
                                            if (!docObject.Sources.Contains<IGH_Param>(current, (IEqualityComparer<IGH_Param>)new GH_Cluster.ParamGuidComparer()))
                                                hookTargetMap.AddMapping((IGH_Param)clusterInputHook, current);
                                        }
                                    }
                                    finally
                                    {
                                        //enumerator2?.Dispose();
                                    }
                                    if (!hookTargetMap.MappingExists((IGH_Param)clusterInputHook))
                                        docObject.RemoveSource((IGH_Param)clusterInputHook);
                                    else
                                        document.AddObject((IGH_DocumentObject)clusterInputHook, false, int.MaxValue);
                                }
                                if (docObject.Recipients.Count != parameter.Recipients.Count)
                                {
                                    GH_ClusterOutputHook clusterOutputHook = new GH_ClusterOutputHook(docObject);
                                    IEnumerator<IGH_Param> enumerator2;
                                    try
                                    {
                                        enumerator2 = parameter.Recipients.GetEnumerator();
                                        while (enumerator2.MoveNext())
                                        {
                                            IGH_Param current = enumerator2.Current;
                                            if (!docObject.Recipients.Contains<IGH_Param>(current, (IEqualityComparer<IGH_Param>)new GH_Cluster.ParamGuidComparer()))
                                                hookTargetMap.AddMapping((IGH_Param)clusterOutputHook, current);
                                        }
                                    }
                                    finally
                                    {
                                        //enumerator2?.Dispose();
                                    }
                                    if (!hookTargetMap.MappingExists((IGH_Param)clusterOutputHook))
                                        clusterOutputHook.RemoveAllSources();
                                    else
                                        document.AddObject((IGH_DocumentObject)clusterOutputHook, false, int.MaxValue);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    //enumerator1.Dispose();
                }
                //document.SetDocumentID(Guid.NewGuid());
                this.CreateFromDocument(document, GH_Convert.ToPoint(@in));
                record.AddAction((IGH_UndoAction)new GH_AddObjectAction((IGH_DocumentObject)this));
                sourceDocument.AddObject((IGH_DocumentObject)this, false, int.MaxValue);
                if (deleteOriginalObjects)
                {
                    GH_UndoRecord removeObjectEvent = sourceDocument.UndoUtil.CreateRemoveObjectEvent("Delete Selection", (IEnumerable<IGH_DocumentObject>)sourceDocument.SelectedObjects());
                    IEnumerator<IGH_UndoAction> enumerator2;
                    try
                    {
                        enumerator2 = removeObjectEvent.Actions.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            IGH_UndoAction current = enumerator2.Current;
                            record.AddAction(current);
                        }
                    }
                    finally
                    {
                        //enumerator2?.Dispose();
                    }
                    sourceDocument.RemoveSelection(false);
                }

                List<IGH_Param>.Enumerator enumerator3;

                if (reconnectSources)
                {
                    try
                    {
                        enumerator3 = this.Params.Input.GetEnumerator();
                        while (enumerator3.MoveNext())
                        {
                            IGH_Param current1 = enumerator3.Current;
                            Guid hook = this.m_mapping.get_Hook(current1);
                            if (!(hook == Guid.Empty))
                            {
                                List<Guid> guidList = hookTargetMap.get_Targets(hook);
                                List<Guid>.Enumerator enumerator2;
                                if (guidList.Count > 0)
                                {
                                    try
                                    {
                                        enumerator2 = guidList.GetEnumerator();
                                        while (enumerator2.MoveNext())
                                        {
                                            Guid current2 = enumerator2.Current;
                                            IGH_Param parameter = sourceDocument.FindParameter(current2);
                                            if (parameter != null)
                                                current1.AddSource(parameter);
                                        }
                                    }
                                    finally
                                    {
                                        //enumerator2.Dispose();
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        //enumerator3.Dispose();
                    }
                }
                List<IGH_Param>.Enumerator enumerator4;
                if (reconnectRecipients)
                {
                    try
                    {
                        enumerator4 = this.Params.Output.GetEnumerator();
                        while (enumerator4.MoveNext())
                        {
                            IGH_Param current1 = enumerator4.Current;
                            Guid hook = this.m_mapping.get_Hook(current1);
                            if (!(hook == Guid.Empty))
                            {
                                List<Guid> guidList = hookTargetMap.get_Targets(hook);
                                List<Guid>.Enumerator enumerator2;
                                if (guidList.Count > 0)
                                {
                                    try
                                    {
                                        enumerator2 = guidList.GetEnumerator();
                                        while (enumerator2.MoveNext())
                                        {
                                            Guid current2 = enumerator2.Current;
                                            sourceDocument.FindParameter(current2)?.AddSource(current1);
                                        }
                                    }
                                    finally
                                    {
                                        //enumerator2.Dispose();
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        //enumerator4.Dispose();
                    }
                }
                sourceDocument.UndoServer.PushUndoRecord(record);
            }
        }

        public void CreateFromFilePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            this.m_file.Path = path;
            switch (this.m_file.PathType)
            {
                case GH_ClusterFileType.None:
                    this.UpdateDocument((GH_Document)null);
                    break;
                case GH_ClusterFileType.Gh:
                case GH_ClusterFileType.Ghx:
                    GH_DocumentIO ghDocumentIo = new GH_DocumentIO();
                    ghDocumentIo.Open(path);
                    if (ghDocumentIo.Document != null && !ghDocumentIo.Document.ContainsClusterHooks())
                        ghDocumentIo.Document.CreateAutomaticClusterHooks();
                    this.UpdateDocument(ghDocumentIo.Document);
                    break;
                case GH_ClusterFileType.GhCluster:
                    this.ReadGhClusterFile(path);
                    break;
            }
            this.m_file.UpdateToCurrentFile();
        }

        public void CreateFromFilePath(string path, System.Drawing.Point location)
        {
            this.CreateFromFilePath(path);
            if (this.m_attributes == null)
                this.CreateAttributes();
            this.Attributes.ExpireLayout();
            this.Attributes.PerformLayout();
            RectangleF bounds = this.Attributes.Bounds;
            float width = (float)location.X - (float)(0.5 * ((double)bounds.Left + (double)bounds.Right));
            float height = (float)location.Y - (float)(0.5 * ((double)bounds.Top + (double)bounds.Bottom));
            IGH_Attributes attributes;
            PointF pointF = (attributes = this.Attributes).Pivot + new SizeF(width, height);
            attributes.Pivot = pointF;
            this.Attributes.ExpireLayout();
        }

        public void UpdateDocument(GH_Document document)
        {
            if (this.m_internalDocument == document)
                return;
            if (this.m_internalDocument != null)
            {
                this.m_internalDocument.Context = GH_DocumentContext.Close;
                this.m_internalDocument.Owner = (IGH_DocumentOwner)null;
                this.m_internalDocument.Nested = false;
                this.m_internalDocument.Dispose();
                this.m_internalDocument = (GH_Document)null;
            }
            this.m_internalDocument = document;
            if (this.m_internalDocument != null)
            {
                this.m_internalDocument.Owner = (IGH_DocumentOwner)this;
                this.m_internalDocument.Nested = true;
                this.m_internalDocument.DeselectAll();
            }
            SortedDictionary<Guid, IGH_Param> table = new SortedDictionary<Guid, IGH_Param>();
            IEnumerator<KeyValuePair<Guid, Guid>> enumerator;
            try
            {
                enumerator = this.m_mapping.ParamToHook.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<Guid, Guid> current = enumerator.Current;
                    IGH_Param ghParam = this.Params.Find(current.Key);
                    if (ghParam != null)
                        table.Add(current.Value, ghParam);
                }
            }
            finally
            {
                //enumerator?.Dispose();
            }
            GH_ComponentParamServer.IGH_SyncObject sync_data = this.Params.EmitSyncObject();
            this.Params.Clear(false);
            this.m_mapping.Clear();
            this.CreateAllInputs(table);
            this.CreateAllOutputs(table);
            this.Params.Sync(sync_data);
            this.ExpireSolution(false);
            this.OnPingDocument()?.DestroyAttributeCache();
        }

        public void ExplodeCluster()
        {
            GH_Document ghDocument = this.OnPingDocument();
            if (ghDocument == null)
                throw new InvalidOperationException("Only clusters already part of a document can be exploded.");
            GH_UndoRecord record = new GH_UndoRecord("Explode Cluster");
            this.DocumentId = Guid.Empty;
            float val1_1 = float.MaxValue;
            float val1_2 = float.MinValue;
            float val1_3 = float.MaxValue;
            float val1_4 = float.MinValue;
            List<Guid> guidList = new List<Guid>();
            IEnumerator<IGH_DocumentObject> enumerator1;
            try
            {
                enumerator1 = this.m_internalDocument.Objects.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    IGH_DocumentObject current = enumerator1.Current;
                    if (!(current.ComponentGuid == GH_ClusterInputHook.ObjID) && !(current.ComponentGuid == GH_ClusterOutputHook.ObjID))
                    {
                        guidList.Add(current.InstanceGuid);
                        val1_1 = Math.Min(val1_1, current.Attributes.Bounds.Left);
                        val1_2 = Math.Max(val1_2, current.Attributes.Bounds.Right);
                        val1_3 = Math.Min(val1_3, current.Attributes.Bounds.Top);
                        val1_4 = Math.Max(val1_4, current.Attributes.Bounds.Bottom);
                    }
                }
            }
            finally
            {
                //enumerator1?.Dispose();
            }
            RectangleF bounds1 = this.Attributes.Bounds;
            float left = bounds1.Left;
            double num1 = Math.Ceiling((double)val1_2 - (double)val1_1);
            bounds1 = this.Attributes.Bounds;
            double width = (double)bounds1.Width;
            int num2 = Math.Max(Convert.ToInt32(num1 - width), 0);
            IEnumerator<IGH_DocumentObject> enumerator2;
            try
            {
                enumerator2 = ghDocument.Objects.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    IGH_DocumentObject current = enumerator2.Current;
                    if (!object.ReferenceEquals((object)current, (object)this) && !(current.ComponentGuid == GH_Group.GroupID))
                    {
                        bounds1 = current.Attributes.Bounds;
                        if ((double)bounds1.Left >= (double)left)
                        {
                            PointF @in = current.Attributes.Pivot;
                            @in.X += (float)num2;
                            @in = (PointF)GH_Convert.ToPoint(@in);
                            record.AddAction((IGH_UndoAction)new GH_PivotAction(current));
                            current.Attributes.Pivot = @in;
                            current.Attributes.ExpireLayout();
                        }
                    }
                }
            }
            finally
            {
                //enumerator2?.Dispose();
            }
            GH_DocumentIO ghDocumentIo = new GH_DocumentIO(this.m_internalDocument);
            ghDocumentIo.Copy(GH_ClipboardType.Local, (IEnumerable<Guid>)guidList);
            ghDocumentIo.Paste(GH_ClipboardType.Local);
            RectangleF bounds2 = this.Attributes.Bounds;
            int int32_1 = Convert.ToInt32(bounds2.Left - val1_1);
            bounds2 = this.Attributes.Bounds;
            double top = (double)bounds2.Top;
            bounds2 = this.Attributes.Bounds;
            double bottom = (double)bounds2.Bottom;
            int int32_2 = Convert.ToInt32(0.5 * (top + bottom) - 0.5 * ((double)val1_3 + (double)val1_4));
            ghDocumentIo.Document.TranslateObjects(new Size(int32_1, int32_2), false);
            ghDocumentIo.Document.DestroyProxySources();
            List<IGH_Param>.Enumerator enumerator3;
            try
            {
                enumerator3 = this.Params.Input.GetEnumerator();
                while (enumerator3.MoveNext())
                {
                    IGH_Param current1 = enumerator3.Current;
                    GH_ClusterInputHook clusterInputHook = this.m_internalDocument.FindObject(this.m_mapping.get_Hook(current1), true) as GH_ClusterInputHook;
                    if (clusterInputHook != null)
                    {
                        List<IGH_Param> ghParamList1 = new List<IGH_Param>((IEnumerable<IGH_Param>)clusterInputHook.Recipients);
                        List<IGH_Param>.Enumerator enumerator4;
                        try
                        {
                            enumerator4 = ghParamList1.GetEnumerator();
                            while (enumerator4.MoveNext())
                            {
                                IGH_Param current2 = enumerator4.Current;
                                IGH_Param parameter = ghDocumentIo.Document.FindParameter(current2.InstanceGuid);
                                if (parameter != null)
                                {
                                    List<IGH_Param> ghParamList2 = new List<IGH_Param>((IEnumerable<IGH_Param>)current1.Sources);
                                    List<IGH_Param>.Enumerator enumerator5;
                                    try
                                    {
                                        enumerator5 = ghParamList2.GetEnumerator();
                                        while (enumerator5.MoveNext())
                                        {
                                            IGH_Param current3 = enumerator5.Current;
                                            parameter.AddSource(current3);
                                        }
                                    }
                                    finally
                                    {
                                        //enumerator5.Dispose();
                                    }
                                }
                            }
                        }
                        finally
                        {
                            //enumerator4.Dispose();
                        }
                    }
                }
            }
            finally
            {
                //enumerator3.Dispose();
            }
            List<IGH_Param>.Enumerator enumerator6;
            try
            {
                enumerator6 = this.Params.Output.GetEnumerator();
                while (enumerator6.MoveNext())
                {
                    IGH_Param current1 = enumerator6.Current;
                    GH_ClusterOutputHook clusterOutputHook = this.m_internalDocument.FindObject(this.m_mapping.get_Hook(current1), true) as GH_ClusterOutputHook;
                    if (clusterOutputHook != null)
                    {
                        List<IGH_Param> ghParamList1 = new List<IGH_Param>((IEnumerable<IGH_Param>)clusterOutputHook.Sources);
                        List<IGH_Param>.Enumerator enumerator4;
                        try
                        {
                            enumerator4 = ghParamList1.GetEnumerator();
                            while (enumerator4.MoveNext())
                            {
                                IGH_Param current2 = enumerator4.Current;
                                IGH_Param parameter = ghDocumentIo.Document.FindParameter(current2.InstanceGuid);
                                if (parameter != null)
                                {
                                    List<IGH_Param> ghParamList2 = new List<IGH_Param>((IEnumerable<IGH_Param>)current1.Recipients);
                                    List<IGH_Param>.Enumerator enumerator5;
                                    try
                                    {
                                        enumerator5 = ghParamList2.GetEnumerator();
                                        while (enumerator5.MoveNext())
                                            enumerator5.Current.ReplaceSource(current1, parameter);
                                    }
                                    finally
                                    {
                                        //enumerator5.Dispose();
                                    }
                                }
                            }
                        }
                        finally
                        {
                            //enumerator4.Dispose();
                        }
                    }
                }
            }
            finally
            {
                //enumerator6.Dispose();
            }
            ghDocumentIo.Document.MutateAllIds();
            IEnumerator<IGH_DocumentObject> enumerator7;
            try
            {
                enumerator7 = ghDocumentIo.Document.Objects.GetEnumerator();
                while (enumerator7.MoveNext())
                {
                    IGH_DocumentObject current = enumerator7.Current;
                    record.AddAction((IGH_UndoAction)new GH_AddObjectAction(current));
                    ghDocument.AddObject(current, false, int.MaxValue);
                }
            }
            finally
            {
                //enumerator7?.Dispose();
            }
            record.AddAction((IGH_UndoAction)new GH_RemoveObjectAction((IGH_DocumentObject)this));
            this.IsolateObject();
            ghDocument.RemoveObject((IGH_DocumentObject)this, false);
            ghDocument.UndoServer.PushUndoRecord(record);
        }

        private void CreateAllInputs(SortedDictionary<Guid, IGH_Param> table)
        {
            if (this.m_internalDocument == null)
                return;
            GH_ClusterInputHook[] clusterInputHookArray1 = this.m_internalDocument.ClusterInputHooks();
            if (clusterInputHookArray1 == null)
                return;
            GH_ClusterInputHook[] clusterInputHookArray2 = clusterInputHookArray1;
            int index = 0;
            while (index < clusterInputHookArray2.Length)
            {
                GH_ClusterInputHook hook = clusterInputHookArray2[index];
                IGH_Param new_param = (IGH_Param)null;
                table.TryGetValue(hook.InstanceGuid, out new_param);
                if (new_param == null)
                    new_param = this.CreateInput(hook);
                this.Params.RegisterInputParam(new_param);
                this.m_mapping.AddMapping(new_param.InstanceGuid, hook.InstanceGuid);
                checked { ++index; }
            }
        }

        private void CreateAllOutputs(SortedDictionary<Guid, IGH_Param> table)
        {
            if (this.m_internalDocument == null)
                return;
            GH_ClusterOutputHook[] clusterOutputHookArray1 = this.m_internalDocument.ClusterOutputHooks();
            if (clusterOutputHookArray1 == null)
                return;
            GH_ClusterOutputHook[] clusterOutputHookArray2 = clusterOutputHookArray1;
            int index = 0;
            while (index < clusterOutputHookArray2.Length)
            {
                GH_ClusterOutputHook hook = clusterOutputHookArray2[index];
                IGH_Param new_param = (IGH_Param)null;
                table.TryGetValue(hook.InstanceGuid, out new_param);
                if (new_param == null)
                    new_param = this.CreateOutput(hook);
                this.Params.RegisterOutputParam(new_param);
                this.m_mapping.AddMapping(new_param, (IGH_Param)hook);
                checked { ++index; }
            }
        }

        private IGH_Param CreateInput(GH_ClusterInputHook hook)
        {
            IGH_Param ghParam = (IGH_Param)null;
            switch (hook.Recipients.Count)
            {
                case 0:
                    if (ghParam == null)
                        ghParam = (IGH_Param)new Param_GenericObject();
                    ghParam.RemoveEffects();
                    ghParam.NewInstanceGuid();
                    ghParam.Name = hook.Name;
                    ghParam.NickName = hook.NickName;
                    ghParam.Description = hook.Description;
                    ghParam.Optional = true;
                    return ghParam;
                case 1:
                    ghParam = GH_ComponentParamServer.CreateDuplicate(hook.Recipients[0]);
                    if (ghParam != null)
                    {
                        ghParam.ClearProxySources();
                        goto case 0;
                    }
                    else
                        goto case 0;
                default:
                    Guid id = GH_Cluster.SharedTypeIDs((IEnumerable<IGH_Param>)hook.Recipients);
                    if (id != Guid.Empty)
                    {
                        IGH_DocumentObject ghDocumentObject = Instances.ComponentServer.EmitObject(id);
                        if (ghDocumentObject != null && ghDocumentObject is IGH_Param)
                        {
                            ghParam = (IGH_Param)ghDocumentObject;
                            ghParam.Name = hook.Recipients[0].Name;
                            ghParam.NickName = hook.Recipients[0].NickName;
                            ghParam.Description = hook.Recipients[0].Description;
                            goto case 0;
                        }
                        else
                            goto case 0;
                    }
                    else
                        goto case 0;
            }
        }

        private IGH_Param CreateOutput(GH_ClusterOutputHook hook)
        {
            IGH_Param ghParam = (IGH_Param)null;
            switch (hook.Sources.Count)
            {
                case 0:
                    if (ghParam == null)
                        ghParam = (IGH_Param)new Param_GenericObject();
                    ghParam.RemoveEffects();
                    ghParam.NewInstanceGuid();
                    ghParam.Name = hook.Name;
                    ghParam.NickName = hook.NickName;
                    ghParam.Description = hook.Description;
                    return ghParam;
                case 1:
                    ghParam = GH_ComponentParamServer.CreateDuplicate(hook.Sources[0]);
                    if (ghParam != null)
                    {
                        ghParam.ClearProxySources();
                        goto case 0;
                    }
                    else
                        goto case 0;
                default:
                    Guid id = GH_Cluster.SharedTypeIDs((IEnumerable<IGH_Param>)hook.Sources);
                    if (id != Guid.Empty)
                    {
                        IGH_DocumentObject ghDocumentObject = Instances.ComponentServer.EmitObject(id);
                        if (ghDocumentObject != null && ghDocumentObject is IGH_Param)
                        {
                            ghParam = (IGH_Param)ghDocumentObject;
                            ghParam.Name = hook.Sources[0].Name;
                            ghParam.NickName = hook.Sources[0].NickName;
                            ghParam.Description = hook.Sources[0].Description;
                            goto case 0;
                        }
                        else
                            goto case 0;
                    }
                    else
                        goto case 0;
            }
        }

        private static Guid SharedTypeIDs(IEnumerable<IGH_Param> @params)
        {
            Guid guid1 = Guid.Empty;
            IEnumerator<IGH_Param> enumerator;
            Guid guid2;
            if (@params != null)
            {
                try
                {
                    enumerator = @params.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        IGH_Param current = enumerator.Current;
                        if (guid1 == Guid.Empty)
                            guid1 = current.ComponentGuid;
                        else if (!(guid1 == current.ComponentGuid))
                        {
                            guid2 = Guid.Empty;
                            goto label_11;
                        }
                    }
                }
                finally
                {
                    //enumerator?.Dispose();
                }
            }
            guid2 = guid1;
        label_11:
            return guid2;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void InstanceGuidsChanged(SortedDictionary<Guid, Guid> map)
        {
            GH_Cluster.HookParamMap hookParamMap = new GH_Cluster.HookParamMap();
            IEnumerator<KeyValuePair<Guid, Guid>> enumerator;
            try
            {
                enumerator = this.m_mapping.ParamToHook.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<Guid, Guid> current = enumerator.Current;
                    Guid key = current.Key;
                    Guid hookId = current.Value;
                    if (map.ContainsKey(key))
                        key = map[key];
                    hookParamMap.AddMapping(key, hookId);
                }
            }
            finally
            {
                //enumerator?.Dispose();
            }
            this.m_mapping = hookParamMap;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DocumentClosed(GH_Document document)
        {
        }

        /// <exclude />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void DocumentModified(GH_Document modifiedDocument)
        {
            if (modifiedDocument == null)
                return;
            Guid id = Guid.NewGuid();
            //modifiedDocument.SetDocumentID(id);
            GH_LooseChunk ghLooseChunk = new GH_LooseChunk("Document");
            modifiedDocument.Write((GH_IWriter)ghLooseChunk);
            GH_Document ghDocument = this.OnPingDocument();
            if (ghDocument == null)
                return;
            ghDocument.DestroyAttributeCache();
            GH_UndoRecord record = new GH_UndoRecord("Cluster Change");
            IEnumerator<GH_Cluster> enumerator1;
            try
            {
                enumerator1 = this.ClusterFamily().GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    GH_Cluster current = enumerator1.Current;
                    record.AddAction((IGH_UndoAction)new GH_GenericObjectAction((IGH_DocumentObject)current));
                    GH_Document document = new GH_Document();
                    document.Read((GH_IReader)ghLooseChunk);
                    current.UpdateDocument(document);
                }
            }
            finally
            {
                //enumerator1?.Dispose();
            }
            this.RecordUndoEvent(record);
            ghDocument.DestroyAttributeCache();
            switch (this.m_file.PathType)
            {
                case GH_ClusterFileType.Gh:
                case GH_ClusterFileType.Ghx:
                    modifiedDocument.FilePath = this.m_file.Path;
                    new GH_DocumentIO(modifiedDocument).Save();
                    IEnumerator<GH_Cluster> enumerator2;
                    try
                    {
                        enumerator2 = this.ClusterFamily().GetEnumerator();
                        while (enumerator2.MoveNext())
                            enumerator2.Current.m_file.UpdateToCurrentFile();
                        break;
                    }
                    finally
                    {
                        //enumerator2?.Dispose();
                    }
                case GH_ClusterFileType.GhCluster:
                    this.WriteGhClusterFile(this.m_file.Path);
                    IEnumerator<GH_Cluster> enumerator3;
                    try
                    {
                        enumerator3 = this.ClusterFamily().GetEnumerator();
                        while (enumerator3.MoveNext())
                            enumerator3.Current.m_file.UpdateToCurrentFile();
                        break;
                    }
                    finally
                    {
                        //enumerator3?.Dispose();
                    }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public GH_Document OwnerDocument()
        {
            return this.OnPingDocument();
        }

        private IEnumerable<GH_Cluster> ClusterFamily()
        {
            List<GH_Cluster> ghClusterList = new List<GH_Cluster>();
            GH_Document ghDocument = this.OnPingDocument();
            if (ghDocument == null || this.DocumentId == Guid.Empty)
            {
                ghClusterList.Add(this);
            }
            else
            {
                //ghClusterList = ghDocument.FindClusters(this.DocumentId);
                if (ghClusterList.Count == 0)
                    ghClusterList.Add(this);
            }
            return (IEnumerable<GH_Cluster>)ghClusterList;
        }

        public GH_ClusterProtection ProtectionLevel
        {
            get
            {
                return this.m_password != null ? (this.m_password.Length != 0 ? GH_ClusterProtection.Protected : GH_ClusterProtection.Unprotected) : GH_ClusterProtection.Unprotected;
            }
        }

        public bool IsPasswordValid(string password)
        {
            return this.m_password == null || (this.m_password.Length == 0 || GH_Cluster.DoKeysMatch(this.m_password, GH_Cluster.CreateKey(password)));
        }

        //public bool RequestPassword(string prompt)
        //{
        //    bool flag;
        //    if (this.ProtectionLevel == GH_ClusterProtection.Unprotected)
        //    {
        //        flag = true;
        //    }
        //    else
        //    {
        //        GH_ClusterPasswordWindow clusterPasswordWindow = new GH_ClusterPasswordWindow();
        //        GH_WindowsFormUtil.CenterFormOnCursor((Form)clusterPasswordWindow, true);
        //        clusterPasswordWindow.NewPasswordGroup.Enabled = false;
        //        clusterPasswordWindow.NewPasswordGroup.Visible = false;
        //        clusterPasswordWindow.OldPassword.Key = this.m_password;
        //        IWin32Window documentEditor = (IWin32Window)Instances.DocumentEditor;
        //        flag = clusterPasswordWindow.ShowDialog(documentEditor) == DialogResult.OK && this.IsPasswordValid(clusterPasswordWindow.OldPassword.Password);
        //    }
        //    return flag;
        //}

        //public bool AssignNewPassword(string oldPassword, string newPassword)
        //{
        //    bool flag;
        //    if (!this.IsPasswordValid(oldPassword))
        //    {
        //        flag = false;
        //    }
        //    else
        //    {
        //        this.m_password = GH_Cluster.CreateKey(newPassword);
        //        flag = true;
        //    }
        //    return flag;
        //}

        internal static byte[] CreateKey(string password)
        {
            byte[] numArray;
            if (string.IsNullOrEmpty(password))
            {
                numArray = (byte[])null;
            }
            else
            {
                password += "salt'n'pepper";
                byte[] hash = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(password));
                if (hash.Length != 16)
                    Array.Resize<byte>(ref hash, 16);
                numArray = hash;
            }
            return numArray;
        }

        public static bool DoKeysMatch(byte[] key0, byte[] key1)
        {
            bool flag;
            if (key0 == null)
                flag = true;
            else if (key0.Length == 0)
                flag = true;
            else if (key1 == null)
                flag = false;
            else if (key1.Length != key0.Length)
            {
                flag = false;
            }
            else
            {
                int num = key0.Length - 1;
                for (int index = 0; index <= num; ++index)
                {
                    if ((int)key0[index] != (int)key1[index])
                    {
                        flag = false;
                        goto label_14;
                    }
                }
                flag = true;
            }
        label_14:
            return flag;
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.DisableGapLogic();
            if (DA.Iteration > 0 || this.m_internalDocument == null)
                return;
            int num1 = this.Params.Input.Count - 1;
            for (int index = 0; index <= num1; ++index)
            {
                IGH_Param ghParam = this.Params.Input[index];
                if (!this.m_mapping.IsParameter(ghParam))
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The input parameters are not synched with the cluster");
                }
                else
                {
                    IGH_Param parameter = this.m_internalDocument.FindParameter(this.m_mapping.get_Hook(ghParam));
                    if (parameter == null)
                    {
                        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Cluster input hook can not be found");
                    }
                    else
                    {
                        parameter.ExpireSolution(false);
                        parameter.ClearData();
                        if (!ghParam.VolatileData.IsEmpty && !parameter.AddVolatileDataTree(ghParam.VolatileData))
                            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Volatile data merge failed");
                    }
                }
            }
            this.m_internalDocument.Enabled = true;
            this.m_internalDocument.NewSolution(false);
            this.m_internalDocument.Enabled = false;
            int num2 = this.Params.Output.Count - 1;
            for (int index = 0; index <= num2; ++index)
            {
                IGH_Param ghParam = this.Params.Output[index];
                if (!this.m_mapping.IsParameter(ghParam))
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The output parameters are not synched with the cluster");
                }
                else
                {
                    IGH_Param parameter = this.m_internalDocument.FindParameter(this.m_mapping.get_Hook(ghParam));
                    if (parameter == null)
                    {
                        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Cluster output hook can not be found");
                    }
                    else
                    {
                        ghParam.VolatileData.Clear();
                        ghParam.AddVolatileDataTree(parameter.VolatileData);
                    }
                }
            }
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            ToolStripMenuItem toolStripMenuItem1 = GH_DocumentObject.Menu_AppendItem((ToolStrip)menu, "Edit Cluster…", new EventHandler(this.MenuEditClusterClicked), this.m_internalDocument != null, false);
            toolStripMenuItem1.ToolTipText = "Open the cluster as a document";
            toolStripMenuItem1.Font = GH_FontServer.NewFont(toolStripMenuItem1.Font, FontStyle.Bold);
            GH_DocumentObject.Menu_AppendItem((ToolStrip)menu, "Properties…", new EventHandler(this.MenuPropertiesClicked)).ToolTipText = "Modify the properties of this cluster";
            //if (this.ProtectionLevel == GH_ClusterProtection.Unprotected)
            //    GH_DocumentObject.Menu_AppendItem((ToolStrip)menu, "Assign Password…", new EventHandler(this.MenuSetPasswordClicked)).ToolTipText = "Protect this cluster with a password";
            //else
            //    GH_DocumentObject.Menu_AppendItem((ToolStrip)menu, "Change Password…", new EventHandler(this.MenuSetPasswordClicked)).ToolTipText = "Change the password of this cluster";
            GH_DocumentObject.Menu_AppendItem((ToolStrip)menu, "Explode Cluster", new EventHandler(this.MenuExplodeClusterClicked), true).ToolTipText = "Explode the cluster and re-insert the contents back into the document";
            ToolStripMenuItem toolStripMenuItem2 = GH_DocumentObject.Menu_AppendItem((ToolStrip)menu, "Preview Contents", new EventHandler(this.MenuPreviewDocumentClicked), true, this.PreviewContent);
            toolStripMenuItem2.ToolTipText = "Draw preview of components inside this cluster";
            if (this.PreviewContent)
            {
                toolStripMenuItem2.ImageScaling = ToolStripItemImageScaling.SizeToFit;
                toolStripMenuItem2.Image = Icons._2;
            }
            else
            {
                toolStripMenuItem2.ImageScaling = ToolStripItemImageScaling.SizeToFit;
                toolStripMenuItem2.Image = Icons._2;
            }
            int num1 = menu.Items.Count - 1;
            for (int index = 0; index <= num1; ++index)
            {
                if (Operators.CompareString(menu.Items[index].Text, "Preview", false) == 0)
                {
                    menu.Items.Remove((ToolStripItem)toolStripMenuItem2);
                    menu.Items.Insert(index + 1, (ToolStripItem)toolStripMenuItem2);
                    break;
                }
            }
            GH_DocumentObject.Menu_AppendSeparator((ToolStrip)menu);
            int num2 = this.ClusterFamily().Count<GH_Cluster>() - 1;
            string str = string.Empty;
            if (num2 > 0)
                str = string.Format(" ({0})", (object)num2);
            ToolStripMenuItem toolStripMenuItem3 = GH_DocumentObject.Menu_AppendItem((ToolStrip)menu, "Disentangle" + str, new EventHandler(this.MenuDisassociatedClicked), num2 > 0);
            if (num2 > 0)
                toolStripMenuItem3.ToolTipText = "Sever ties between this instance and any other" + Environment.NewLine + "instances of this cluster within the document.";
            else
                toolStripMenuItem3.ToolTipText = "This cluster is not entangled with any other clusters.";
            GH_DocumentObject.Menu_AppendItem((ToolStrip)menu, "Export…", new EventHandler(this.MenuExportClicked), Icons._3, this.m_internalDocument != null, false).ToolTipText = "Create a new *.ghcluster file based on this cluster.";
            GH_DocumentObject.Menu_AppendItem((ToolStrip)menu, "Export && Reference…", new EventHandler(this.MenuExportAndLinkClicked), Icons._3, this.m_internalDocument != null, false).ToolTipText = "Create a new *.ghcluster file based on this cluster" + Environment.NewLine + "and immediately link to that file.";
            switch (this.m_file.Synchronization)
            {
                case GH_Synchronisation.Unset:
                    GH_DocumentObject.Menu_AppendItem((ToolStrip)menu, "Update", (EventHandler)null, false);
                    break;
                case GH_Synchronisation.NotReferenced:
                    GH_DocumentObject.Menu_AppendItem((ToolStrip)menu, "Update", (EventHandler)null, false).ToolTipText = "This cluster is not referenced and therefore cannot be updated.";
                    break;
                case GH_Synchronisation.MissingReference:
                    GH_DocumentObject.Menu_AppendItem((ToolStrip)menu, "Update", new EventHandler(this.MenuUpdateReferenceClicked), true).ToolTipText = "This cluster targets a reference file which no longer exists." + Environment.NewLine + "Click here to target a different file.";
                    break;
                case GH_Synchronisation.UpToDate:
                    GH_DocumentObject.Menu_AppendItem((ToolStrip)menu, "Update", (EventHandler)null, false).ToolTipText = "This cluster is already up to date.";
                    break;
                case GH_Synchronisation.OutOfDate:
                    GH_DocumentObject.Menu_AppendItem((ToolStrip)menu, "Update", new EventHandler(this.MenuUpdateReferenceClicked), true).ToolTipText = "Update the cluster to the most recent version of the referenced file.";
                    break;
            }
            GH_DocumentObject.Menu_AppendItem((ToolStrip)menu, "Internalise", new EventHandler(this.MenuDereferenceClicked), this.m_file.IsPath).ToolTipText = "Dereference this cluster from the target file and make it local.";
        }

        private void MenuPropertiesClicked(object sender, EventArgs e)
        {
            this.DisplayClusterPropertiesEditor();
        }

        private void MenuExplodeClusterClicked(object sender, EventArgs e)
        {
            //if (!this.RequestPassword("Enter the cluster password:"))
            //    return;
            //GH_Document ghDocument = this.OnPingDocument();
            //if (ghDocument == null)
            //    return;
            //this.ExplodeCluster();
            //Instances.InvalidateCanvas();
            //ghDocument.NewSolution(false);
        }

        private void MenuSetPasswordClicked(object sender, EventArgs e)
        {
            //    GH_ClusterPasswordWindow clusterPasswordWindow = new GH_ClusterPasswordWindow();
            //    GH_WindowsFormUtil.CenterFormOnCursor((Form)clusterPasswordWindow, true);
            //    if (this.ProtectionLevel == GH_ClusterProtection.Protected)
            //        clusterPasswordWindow.OldPassword.Key = this.m_password;
            //    else
            //        clusterPasswordWindow.OldPasswordGroup.Enabled = false;
            //    if (clusterPasswordWindow.ShowDialog((IWin32Window)Instances.DocumentEditor) != DialogResult.OK)
            //        return;
            //    byte[] key1 = GH_Cluster.CreateKey(clusterPasswordWindow.OldPassword.Password);
            //    byte[] key2 = GH_Cluster.CreateKey(clusterPasswordWindow.NewPassword.Password);
            //    if (!GH_Cluster.DoKeysMatch(this.m_password, key1))
            //        return;
            //    GH_UndoRecord record = new GH_UndoRecord("Cluster Password");
            //    Guid guid = Guid.NewGuid();
            //    IEnumerator<GH_Cluster> enumerator;
            //    try
            //    {
            //        enumerator = this.ClusterFamily().GetEnumerator();
            //        while (enumerator.MoveNext())
            //        {
            //            GH_Cluster current = enumerator.Current;
            //            record.AddAction((IGH_UndoAction)new GH_Cluster.GH_ClusterPasswordUndoAction(current));
            //            record.AddAction((IGH_UndoAction)new GH_Cluster.GH_ClusterDocumentIdAction(current));
            //            current.m_password = key2;
            //            current.DocumentId = guid;
            //        }
            //    }
            //    finally
            //    {
            //        //enumerator?.Dispose();
            //    }
            //    this.RecordUndoEvent(record);
        }

        private void MenuEditClusterClicked(object sender, EventArgs e)
        {
            this.EditClusterAsSeparateDocument();
        }

        private void MenuPreviewDocumentClicked(object sender, EventArgs e)
        {
            this.RecordUndoEvent("Preview Content", (IGH_UndoAction)new GH_Cluster.GH_ClusterPreviewDocumentAction(this));
            this.PreviewContent = !this.PreviewContent;
            this.OnPreviewExpired(true);
        }

        private void MenuDisassociatedClicked(object sender, EventArgs e)
        {
            if (this.m_internalDocument == null)
                return;
            this.RecordUndoEvent("Disassociate Cluster", (IGH_UndoAction)new GH_Cluster.GH_ClusterDocumentIdAction(this));
            //this.m_internalDocument.SetDocumentID(Guid.NewGuid());
        }

        private void MenuExportClicked(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Export Cluster";
            saveFileDialog.FileName = this.NickName;
            saveFileDialog.Filter = "Grasshopper Clusters (*.ghcluster)|*.ghcluster";
            if (saveFileDialog.ShowDialog((IWin32Window)Instances.DocumentEditor) != DialogResult.OK)
                return;
            try
            {
                this.WriteGhClusterFile(saveFileDialog.FileName);
            }
            catch (Exception ex)
            {
                //ProjectData.SetProjectError(ex);
                Tracing.Assert(new Guid("{17F532BB-3384-42ee-9460-6F417934D286}"), "Cluster export failed", ex);
                //ProjectData.ClearProjectError();
            }
        }

        private void MenuExportAndLinkClicked(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Export Cluster";
            saveFileDialog.FileName = this.NickName;
            saveFileDialog.Filter = "Grasshopper Clusters (*.ghcluster)|*.ghcluster";
            if (saveFileDialog.ShowDialog((IWin32Window)Instances.DocumentEditor) != DialogResult.OK)
                return;
            try
            {
                string fileName = saveFileDialog.FileName;
                Guid guid = Guid.NewGuid();
                this.DocumentId = guid;
                this.WriteGhClusterFile(fileName);
                GH_UndoRecord record = new GH_UndoRecord("Plünk Cluster");
                IEnumerator<GH_Cluster> enumerator;
                try
                {
                    enumerator = this.ClusterFamily().GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        GH_Cluster current = enumerator.Current;
                        record.AddAction((IGH_UndoAction)new GH_Cluster.GH_ClusterReferenceAction(current));
                        record.AddAction((IGH_UndoAction)new GH_Cluster.GH_ClusterDocumentIdAction(current));
                        current.DocumentId = guid;
                        current.m_file.Path = fileName;
                        current.m_file.UpdateToCurrentFile();
                    }
                }
                finally
                {
                    //enumerator?.Dispose();
                }
                this.RecordUndoEvent(record);
                Instances.InvalidateCanvas();
            }
            catch (Exception ex)
            {
                //ProjectData.SetProjectError(ex);
                Tracing.Assert(new Guid("{17F532BB-3384-42ee-9460-6F417934D286}"), "Cluster export failed", ex);
                //ProjectData.ClearProjectError();
            }
        }

        private void MenuUpdateReferenceClicked(object sender, EventArgs e)
        {
            switch (this.m_file.Synchronization)
            {
                case GH_Synchronisation.MissingReference:
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Title = "Cluster reference file";
                    openFileDialog.Filter = "Grasshopper Clusters (*.ghcluster)|*.ghcluster|Grasshopper Files (*.gh;*.ghx)|*.gh;*.ghx";
                    if (openFileDialog.ShowDialog((IWin32Window)Instances.DocumentEditor) != DialogResult.OK)
                        break;
                    GH_UndoRecord record1 = new GH_UndoRecord("Reference Cluster");
                    IEnumerator<GH_Cluster> enumerator1;
                    try
                    {
                        enumerator1 = this.ClusterFamily().GetEnumerator();
                        while (enumerator1.MoveNext())
                        {
                            GH_Cluster current = enumerator1.Current;
                            record1.AddAction((IGH_UndoAction)new GH_GenericObjectAction((IGH_DocumentObject)current));
                            current.CreateFromFilePath(openFileDialog.FileName);
                        }
                    }
                    finally
                    {
                        //enumerator1?.Dispose();
                    }
                    this.RecordUndoEvent(record1);
                    this.ExpireSolution(true);
                    break;
                case GH_Synchronisation.OutOfDate:
                    GH_UndoRecord record2 = new GH_UndoRecord("Update Cluster");
                    IEnumerator<GH_Cluster> enumerator2;
                    try
                    {
                        enumerator2 = this.ClusterFamily().GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            GH_Cluster current = enumerator2.Current;
                            record2.AddAction((IGH_UndoAction)new GH_GenericObjectAction((IGH_DocumentObject)current));
                            current.CreateFromFilePath(current.m_file.Path);
                        }
                    }
                    finally
                    {
                        //enumerator2?.Dispose();
                    }
                    this.RecordUndoEvent(record2);
                    this.ExpireSolution(true);
                    break;
            }
        }

        private void MenuDereferenceClicked(object sender, EventArgs e)
        {
            Guid guid = Guid.NewGuid();
            GH_UndoRecord record = new GH_UndoRecord("Cluster Frabbing");
            IEnumerator<GH_Cluster> enumerator;
            try
            {
                enumerator = this.ClusterFamily().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    GH_Cluster current = enumerator.Current;
                    record.AddAction((IGH_UndoAction)new GH_Cluster.GH_ClusterReferenceAction(current));
                    record.AddAction((IGH_UndoAction)new GH_Cluster.GH_ClusterDocumentIdAction(current));
                    current.DocumentId = guid;
                    current.m_file.Path = (string)null;
                    current.m_file.UpdateToCurrentFile();
                }
            }
            finally
            {
                //enumerator?.Dispose();
            }
            this.RecordUndoEvent(record);
            Instances.InvalidateCanvas();
        }

        public void EditClusterAsSeparateDocument()
        {
            if (this.m_internalDocument == null)
                return;
            GH_Canvas activeCanvas = Instances.ActiveCanvas;
            if (activeCanvas == null)
                return;
            try
            {
                foreach (GH_Document ghDocument in Instances.DocumentServer)
                {
                    if (ghDocument.DocumentID == this.m_internalDocument.DocumentID)
                    {
                        if (ghDocument.Owner != this && ghDocument.Owner == null)
                            ghDocument.Owner = (IGH_DocumentOwner)this;
                        activeCanvas.Document = ghDocument;
                        ghDocument.NewSolution(false);
                        return;
                    }
                }
            }
            finally
            {
                //IEnumerator enumerator;
                //if (enumerator is IDisposable)
                //    (enumerator as IDisposable).Dispose();
            }
            //if (!this.RequestPassword("Enter the cluster password:"))
            //    return;
            GH_Document document = GH_Document.DuplicateDocument(this.m_internalDocument);
            document.Owner = (IGH_DocumentOwner)this;
            IEnumerator<KeyValuePair<Guid, Guid>> enumerator1;
            try
            {
                enumerator1 = this.m_mapping.ParamToHook.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    KeyValuePair<Guid, Guid> current = enumerator1.Current;
                    IGH_Param ghParam = this.Params.Find(current.Key);
                    GH_ClusterInputHook clusterInputHook = document.FindObject<GH_ClusterInputHook>(current.Value, true);
                    if (ghParam != null && clusterInputHook != null)
                    {
                        GH_Structure<IGH_Goo> data1 = new GH_Structure<IGH_Goo>();
                        int num = ghParam.VolatileData.PathCount - 1;
                        for (int index = 0; index <= num; ++index)
                        {
                            GH_Path path = ghParam.VolatileData.get_Path(index);
                            IList list = ghParam.VolatileData.get_Branch(index);
                            try
                            {
                                foreach (IGH_Goo data2 in (IEnumerable)list)
                                    data1.Append(data2, path);
                            }
                            finally
                            {
                                //IEnumerator enumerator2;
                                //if (enumerator2 is IDisposable)
                                //    (enumerator2 as IDisposable).Dispose();
                            }
                            clusterInputHook.SetPlaceholderData(data1);
                        }
                    }
                }
            }
            finally
            {
                //enumerator1?.Dispose();
            }
            Instances.DocumentServer.AddDocument(document);
            activeCanvas.Document = document;
            Rectangle screenPort = activeCanvas.Viewport.ScreenPort;
            Rectangle rectangle = GH_Convert.ToRectangle(document.BoundingBox(false));
            rectangle.Inflate(5, 5);
            screenPort.Inflate(-5, -5);
            new GH_NamedView(screenPort, (RectangleF)rectangle).SetToViewport(activeCanvas, 250);
            document.NewSolution(false);
        }

        public void DisplayClusterPropertiesEditor()
        {
            //if (!this.RequestPassword("Enter the cluster password to gain access to the cluster properties"))
            //    return;
            GH_ClusterPropertiesEditor propertiesEditor = new GH_ClusterPropertiesEditor();
            propertiesEditor.LoadCluster(this);
            GH_WindowsFormUtil.CenterFormOnCursor((Form)propertiesEditor, true);
            if (propertiesEditor.ShowDialog((IWin32Window)Instances.DocumentEditor) != DialogResult.OK)
                return;
            GH_UndoRecord record = new GH_UndoRecord("Cluster Properties");
            Guid guid = Guid.NewGuid();
            IEnumerator<GH_Cluster> enumerator;
            try
            {
                enumerator = this.ClusterFamily().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    GH_Cluster current = enumerator.Current;
                    record.AddAction((IGH_UndoAction)new GH_Cluster.GH_ClusterPropertiesUndoAction(current));
                    record.AddAction((IGH_UndoAction)new GH_Cluster.GH_ClusterDocumentIdAction(current));
                    propertiesEditor.WriteToCluster(current);
                    current.DocumentId = guid;
                    current.Attributes.ExpireLayout();
                }
            }
            finally
            {
                //enumerator?.Dispose();
            }
            this.RecordUndoEvent(record);
            Instances.RedrawCanvas();
        }

        public bool PreviewContent
        {
            get
            {
                return this.m_previewContent;
            }
            set
            {
                this.m_previewContent = value;
            }
        }

        public override bool IsPreviewCapable
        {
            get
            {
                bool isPreviewCapable = base.IsPreviewCapable;
                bool flag;
                if (this.m_internalDocument == null)
                {
                    flag = isPreviewCapable;
                }
                else
                {
                    IEnumerator<IGH_DocumentObject> enumerator;
                    if (this.PreviewContent)
                    {
                        try
                        {
                            enumerator = this.m_internalDocument.Objects.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                IGH_PreviewObject current = enumerator.Current as IGH_PreviewObject;
                                if (current != null && current.IsPreviewCapable)
                                {
                                    flag = true;
                                    goto label_11;
                                }
                            }
                        }
                        finally
                        {
                            //enumerator?.Dispose();
                        }
                    }
                    flag = isPreviewCapable;
                }
            label_11:
                return flag;
            }
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            base.DrawViewportWires(args);
            if (!this.PreviewContent || this.m_internalDocument == null)
                return;
            Color color;
            DisplayMaterial displayMaterial;
            if (this.Attributes.Selected)
            {
                color = args.WireColour_Selected;
                displayMaterial = args.ShadeMaterial_Selected;
            }
            else
            {
                color = args.WireColour;
                displayMaterial = args.ShadeMaterial;
            }
            GH_PreviewArgs ghPreviewArgs = new GH_PreviewArgs(this.m_internalDocument, args.Display, args.Viewport, args.DefaultCurveThickness, color, color, displayMaterial, displayMaterial, args.MeshingParameters);
            IEnumerator<IGH_DocumentObject> enumerator;
            try
            {
                enumerator = this.m_internalDocument.Objects.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    IGH_PreviewObject current = enumerator.Current as IGH_PreviewObject;
                    if (current != null && !current.Hidden)
                        current.DrawViewportWires((IGH_PreviewArgs)ghPreviewArgs);
                }
            }
            finally
            {
                //enumerator?.Dispose();
            }
        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            base.DrawViewportMeshes(args);
            if (!this.PreviewContent || this.m_internalDocument == null)
                return;
            Color color;
            DisplayMaterial displayMaterial;
            if (this.Attributes.Selected)
            {
                color = args.WireColour_Selected;
                displayMaterial = args.ShadeMaterial_Selected;
            }
            else
            {
                color = args.WireColour;
                displayMaterial = args.ShadeMaterial;
            }
            GH_PreviewArgs ghPreviewArgs = new GH_PreviewArgs(this.m_internalDocument, args.Display, args.Viewport, args.DefaultCurveThickness, color, color, displayMaterial, displayMaterial, args.MeshingParameters);
            IEnumerator<IGH_DocumentObject> enumerator;
            try
            {
                enumerator = this.m_internalDocument.Objects.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    IGH_PreviewObject current = enumerator.Current as IGH_PreviewObject;
                    if (current != null && !current.Hidden)
                        current.DrawViewportMeshes((IGH_PreviewArgs)ghPreviewArgs);
                }
            }
            finally
            {
                //enumerator?.Dispose();
            }
        }

        public override BoundingBox ClippingBox
        {
            get
            {
                BoundingBox clippingBox = base.ClippingBox;
                IEnumerator<IGH_DocumentObject> enumerator;
                if (this.PreviewContent)
                {
                    if (this.m_internalDocument != null)
                    {
                        try
                        {
                            enumerator = this.m_internalDocument.Objects.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                IGH_PreviewObject current = enumerator.Current as IGH_PreviewObject;
                                if (current != null && !current.Hidden)
                                    ((BoundingBox)clippingBox).Union(current.ClippingBox);
                            }
                        }
                        finally
                        {
                            //enumerator?.Dispose();
                        }
                    }
                }
                return clippingBox;
            }
        }

        public override bool IsBakeCapable
        {
            get
            {
                bool flag;
                if (this.m_internalDocument == null)
                {
                    flag = false;
                }
                else
                {
                    IEnumerator<IGH_DocumentObject> enumerator;
                    try
                    {
                        enumerator = this.m_internalDocument.Objects.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            IGH_BakeAwareObject current = enumerator.Current as IGH_BakeAwareObject;
                            if (current != null && current.IsBakeCapable)
                            {
                                flag = true;
                                goto label_10;
                            }
                        }
                    }
                    finally
                    {
                        //enumerator?.Dispose();
                    }
                    flag = false;
                }
            label_10:
                return flag;
            }
        }

        public override void BakeGeometry(RhinoDoc doc, List<Guid> objectIds)
        {
            this.BakeGeometry(doc, (ObjectAttributes)null, objectIds);
        }

        public override void BakeGeometry(RhinoDoc doc, ObjectAttributes att, List<Guid> objectIds)
        {
            if (this.m_internalDocument == null)
                return;
            IEnumerator<IGH_DocumentObject> enumerator;
            if (this.PreviewContent)
            {
                try
                {
                    enumerator = this.m_internalDocument.Objects.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        IGH_DocumentObject current = enumerator.Current;
                        IGH_BakeAwareObject ghBakeAwareObject = current as IGH_BakeAwareObject;
                        if (ghBakeAwareObject != null)
                        {
                            IGH_PreviewObject ghPreviewObject = current as IGH_PreviewObject;
                            if (ghPreviewObject == null || !ghPreviewObject.Hidden)
                                ghBakeAwareObject.BakeGeometry(doc, att, objectIds);
                        }
                    }
                }
                finally
                {
                    //enumerator?.Dispose();
                }
            }
            else
                base.BakeGeometry(doc, att, objectIds);
        }

        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            return false;
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            return false;
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            return (IGH_Param)null;
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            return false;
        }

        public void VariableParameterMaintenance()
        {
            List<IGH_Param>.Enumerator enumerator1;
            try
            {
                enumerator1 = this.Params.Input.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    IGH_Param current = enumerator1.Current;
                    current.Optional = true;
                    current.MutableNickName = true;
                }
            }
            finally
            {
                //enumerator1.Dispose();
            }
            List<IGH_Param>.Enumerator enumerator2;
            try
            {
                enumerator2 = this.Params.Output.GetEnumerator();
                while (enumerator2.MoveNext())
                    enumerator2.Current.MutableNickName = true;
            }
            finally
            {
                //enumerator2.Dispose();
            }
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("PreviewDocument", this.PreviewContent);
            if (this.Author != null && !this.Author.IsEmpty)
                ((GH_Author)this.Author).Write(writer.CreateChunk("Author"));
            if (this.m_file.IsPath)
                this.m_file.Write(writer.CreateChunk("FileInfo"));
            if (this.m_password != null && this.m_password.Length > 0)
                writer.SetByteArray("Key", this.m_password);
            if (this.m_internalDocument != null)
            {
                this.m_mapping.Write(writer.CreateChunk("ParamMap"));
                GH_LooseChunk ghLooseChunk = new GH_LooseChunk("Document");
                this.m_internalDocument.Write((GH_IWriter)ghLooseChunk);
                byte[] numArray1 = ghLooseChunk.Serialize_Binary();
                if (numArray1 != null)
                {
                    if (this.ProtectionLevel == GH_ClusterProtection.Unprotected)
                    {
                        writer.SetByteArray("ClusterDocument", numArray1);
                    }
                    else
                    {
                        byte[] numArray2 = this.EncryptByteArray(numArray1, this.m_password);
                        if (numArray2 == null)
                        {
                            writer.AddMessage("Cluster encryption failed. Cluster will be stored without encryption", GH_Message_Type.error);
                            writer.SetByteArray("ClusterDocument", numArray1);
                        }
                        else
                        {
                            byte[] item_value = numArray2;
                            writer.SetByteArray("EncryptedDocument", item_value);
                        }
                    }
                }
            }
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            if (this.m_internalDocument != null)
            {
                this.m_internalDocument.Context = GH_DocumentContext.Close;
                this.m_internalDocument.Owner = (IGH_DocumentOwner)null;
                this.m_internalDocument.Nested = false;
                this.m_internalDocument.Dispose();
                this.m_internalDocument = (GH_Document)null;
            }
            if (this.m_attributes == null)
                this.CreateAttributes();
            bool flag;
            if (!base.Read(reader))
            {
                flag = false;
            }
            else
            {
                this.PreviewContent = false;
                reader.TryGetBoolean("PreviewDocument", ref this.m_previewContent);
                GH_Author ghAuthor = new GH_Author();
                ghAuthor.Clear();
                GH_IReader chunk1 = reader.FindChunk("Author");
                if (chunk1 != null)
                    ghAuthor.Read(chunk1);
                this.Author = (IGH_Author)ghAuthor;
                this.m_password = (byte[])null;
                if (reader.ItemExists("Key"))
                    this.m_password = reader.GetByteArray("Key");
                this.m_file.Clear();
                GH_IReader chunk2 = reader.FindChunk("FileInfo");
                if (chunk2 != null)
                    this.m_file.Read(chunk2);
                this.m_mapping.Clear();
                this.m_internalDocument = (GH_Document)null;
                byte[] content = (byte[])null;
                if (reader.ItemExists("ClusterDocument"))
                    content = reader.GetByteArray("ClusterDocument");
                else if (reader.ItemExists("EncryptedDocument"))
                    content = this.DecryptByteArray(reader.GetByteArray("EncryptedDocument"), this.m_password);
                if (content != null)
                {
                    GH_LooseChunk ghLooseChunk = new GH_LooseChunk("Document");
                    try
                    {
                        ghLooseChunk.Deserialize_Binary(content);
                        GH_Document document = new GH_Document();
                        if (document.Read((GH_IReader)ghLooseChunk))
                        {
                            GH_IReader chunk3 = reader.FindChunk("ParamMap");
                            if (chunk3 != null)
                                this.m_mapping.Read(chunk3);
                            this.UpdateDocument(document);
                        }
                        else
                            reader.AddMessage("Encrypted document failed to deserialize", GH_Message_Type.error);
                    }
                    catch (Exception ex)
                    {
                        //ProjectData.SetProjectError(ex);
                        reader.AddMessage("Encrypted cluster failed to deserialize", GH_Message_Type.error);
                        //ProjectData.ClearProjectError();
                    }
                }
                flag = true;
            }
            return flag;
        }

        private byte[] EncryptByteArray(byte[] data, byte[] key)
        {
            return data != null ? (key != null ? (data.Length != 0 ? (key.Length != 0 ? this.CreateEncryptor(key).CreateEncryptor().TransformFinalBlock(data, 0, data.Length) : data) : data) : data) : data;
        }

        private byte[] DecryptByteArray(byte[] data, byte[] key)
        {
            return data != null ? (key != null ? (data.Length != 0 ? (key.Length != 0 ? this.CreateEncryptor(key).CreateDecryptor().TransformFinalBlock(data, 0, data.Length) : data) : data) : data) : data;
        }

        private RijndaelManaged CreateEncryptor(byte[] key)
        {
            byte[] numArray = new byte[16];
            int index1 = -1;
            int num = numArray.Length - 1;
            for (int index2 = 0; index2 <= num; ++index2)
            {
                ++index1;
                if (index1 >= key.Length)
                    index1 = 0;
                numArray[index2] = key[index1];
            }
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.KeySize = 128;
            rijndaelManaged.BlockSize = 128;
            rijndaelManaged.Key = numArray;
            rijndaelManaged.IV = numArray;
            rijndaelManaged.Mode = CipherMode.CBC;
            rijndaelManaged.Padding = PaddingMode.PKCS7;
            return rijndaelManaged;
        }

        public bool WriteGhClusterFile(string path)
        {
            bool flag;
            try
            {
                GH_LooseChunk ghLooseChunk = new GH_LooseChunk("Cluster");
                if (!this.Write((GH_IWriter)ghLooseChunk))
                {
                    flag = false;
                }
                else
                {
                    File.WriteAllBytes(path, ghLooseChunk.Serialize_Binary());
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                //ProjectData.SetProjectError(ex);
                flag = false;
                //ProjectData.ClearProjectError();
            }
            return flag;
        }

        public bool ReadGhClusterFile(string path)
        {
            bool flag;
            try
            {
                GH_LooseChunk ghLooseChunk = new GH_LooseChunk("Cluster");
                ghLooseChunk.Deserialize_Binary(File.ReadAllBytes(path));
                GH_Cluster ghCluster = new GH_Cluster();
                if (!ghCluster.Read((GH_IReader)ghLooseChunk))
                {
                    flag = false;
                }
                else
                {
                    this.m_author = ghCluster.Author;
                    this.m_password = ghCluster.m_password;
                    GH_Document internalDocument = ghCluster.m_internalDocument;
                    ghCluster.m_mapping = (GH_Cluster.HookParamMap)null;
                    ghCluster.m_internalDocument = (GH_Document)null;
                    ghCluster.m_file.Path = (string)null;
                    if (internalDocument != null)
                        this.UpdateDocument(internalDocument);
                    this.m_file.Path = path;
                    this.m_file.UpdateToCurrentFile();
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                //ProjectData.SetProjectError(ex);
                flag = false;
                //ProjectData.ClearProjectError();
            }
            return flag;
        }

        private class HookParamMap : GH_ISerializable
        {
            private readonly SortedDictionary<Guid, Guid> m_param;
            private readonly SortedDictionary<Guid, Guid> m_hook;

            public HookParamMap()
            {
                this.m_param = new SortedDictionary<Guid, Guid>();
                this.m_hook = new SortedDictionary<Guid, Guid>();
            }

            /// <summary>Remove all parameter|hook pairs.</summary>
            public void Clear()
            {
                this.m_param.Clear();
                this.m_hook.Clear();
            }

            /// <summary>Add another parameter|hook pair.</summary>
            public void AddMapping(Guid paramId, Guid hookId)
            {
                this.m_param.Remove(paramId);
                this.m_hook.Remove(hookId);
                this.m_param.Add(paramId, hookId);
                this.m_hook.Add(hookId, paramId);
            }

            /// <summary>Add another parameter|hook pair.</summary>
            public void AddMapping(IGH_Param param, IGH_Param hookId)
            {
                this.AddMapping(param.InstanceGuid, hookId.InstanceGuid);
            }

            /// <summary>Remove a parameter|hook pair.</summary>
            public void RemoveMapping(Guid param, Guid hookId)
            {
                if (!this.m_param.ContainsKey(param) || !this.m_hook.ContainsKey(hookId) || (!(this.m_param[param] == hookId) || !(this.m_hook[hookId] == param)))
                    return;
                this.m_param.Remove(param);
                this.m_hook.Remove(hookId);
            }

            public Guid get_Parameter(Guid hookId)
            {
                return this.m_hook.ContainsKey(hookId) ? this.m_hook[hookId] : Guid.Empty;
            }

            public Guid get_Parameter(IGH_Param hookParam)
            {
                return this.get_Parameter(hookParam.InstanceGuid);
            }

            public Guid get_Hook(Guid param)
            {
                return this.m_param.ContainsKey(param) ? this.m_param[param] : Guid.Empty;
            }

            public Guid get_Hook(IGH_Param param)
            {
                return this.get_Hook(param.InstanceGuid);
            }

            /// <summary>Gets whether the parameter is defined in the map.</summary>
            public bool IsParameter(Guid id)
            {
                return this.m_param.ContainsKey(id);
            }

            /// <summary>Gets whether the parameter is defined in the map.</summary>
            public bool IsParameter(IGH_Param param)
            {
                return this.IsParameter(param.InstanceGuid);
            }

            /// <summary>Gets whether the hook is defined in the map.</summary>
            public bool IsHook(Guid id)
            {
                return this.m_hook.ContainsKey(id);
            }

            /// <summary>Gets whether the hook is defined in the map.</summary>
            public bool IsHook(IGH_Param hookParam)
            {
                return this.IsHook(hookParam.InstanceGuid);
            }

            /// <summary>
            /// Gets the parameter to hook mapping. Parameter ids are key, hook ids are value.
            /// </summary>
            public IEnumerable<KeyValuePair<Guid, Guid>> ParamToHook
            {
                get
                {
                    return (IEnumerable<KeyValuePair<Guid, Guid>>)this.m_param;
                }
            }

            /// <summary>
            /// Gets the parameter to hook mapping. Parameter ids are key, hook ids are value.
            /// </summary>
            public IEnumerable<KeyValuePair<Guid, Guid>> HookToParam
            {
                get
                {
                    return (IEnumerable<KeyValuePair<Guid, Guid>>)this.m_hook;
                }
            }

            /// <summary>Serialize the mapping to an archive.</summary>
            public bool Write(GH_IWriter writer)
            {
                writer.SetInt32("Count", this.m_param.Count);
                int item_index = -1;
                SortedDictionary<Guid, Guid>.Enumerator enumerator;
                try
                {
                    enumerator = this.m_param.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<Guid, Guid> current = enumerator.Current;
                        ++item_index;
                        writer.SetGuid("Key", item_index, current.Key);
                        writer.SetGuid("Value", item_index, current.Value);
                    }
                }
                finally
                {
                    //enumerator.Dispose();
                }
                return true;
            }

            /// <summary>Deserialize the mapping from an archive.</summary>
            public bool Read(GH_IReader reader)
            {
                this.Clear();
                int num1 = 0;
                reader.TryGetInt32("Count", ref num1);
                int num2 = num1 - 1;
                for (int item_index = 0; item_index <= num2; ++item_index)
                    this.AddMapping(reader.GetGuid("Key", item_index), reader.GetGuid("Value", item_index));
                return true;
            }
        }

        private class HookTargetMap
        {
            private readonly SortedDictionary<Guid, List<Guid>> m_map;

            public HookTargetMap()
            {
                this.m_map = new SortedDictionary<Guid, List<Guid>>();
            }

            /// <summary>Remove all mappings.</summary>
            public void Clear()
            {
                this.m_map.Clear();
            }

            /// <summary>Add a new hook|target mapping.</summary>
            public void AddMapping(Guid hook, Guid target)
            {
                List<Guid> guidList = (List<Guid>)null;
                if (this.m_map.TryGetValue(hook, out guidList))
                {
                    if (guidList.Contains(target))
                        return;
                    guidList.Add(target);
                }
                else
                    this.m_map.Add(hook, new List<Guid>() { target });
            }

            /// <summary>Add a new hook|target mapping.</summary>
            public void AddMapping(IGH_Param hook, IGH_Param target)
            {
                this.AddMapping(hook.InstanceGuid, target.InstanceGuid);
            }

            /// <summary>Remove all mappings concerning a specific hook.</summary>
            public void RemoveMapping(Guid hook)
            {
                if (!this.m_map.ContainsKey(hook))
                    return;
                this.m_map.Remove(hook);
            }

            /// <summary>Remove all mappings concerning a specific hook.</summary>
            public void RemoveMapping(IGH_Param hook)
            {
                this.RemoveMapping(hook.InstanceGuid);
            }

            /// <summary>Remove a specific mapping.</summary>
            public void RemoveMapping(Guid hook, Guid target)
            {
                if (!this.m_map.ContainsKey(hook))
                    return;
                List<Guid> guidList = this.m_map[hook];
                if (!guidList.Contains(target))
                    return;
                if (guidList.Count < 2)
                    this.m_map.Remove(hook);
                else
                    guidList.Remove(target);
            }

            /// <summary>Remove a specific mapping.</summary>
            public void RemoveMapping(IGH_Param hook, IGH_Param target)
            {
                this.RemoveMapping(hook.InstanceGuid, target.InstanceGuid);
            }

            public List<Guid> get_Targets(Guid hook)
            {
                return !this.m_map.ContainsKey(hook) ? new List<Guid>() : this.m_map[hook];
            }

            public List<Guid> get_Targets(IGH_Param hook)
            {
                return this.get_Targets(hook.InstanceGuid);
            }

            public bool IsMapping(Guid hook)
            {
                return this.m_map.ContainsKey(hook);
            }

            public bool MappingExists(IGH_Param hook)
            {
                return this.IsMapping(hook.InstanceGuid);
            }
        }

        private class ParamGuidComparer : IEqualityComparer<IGH_Param>
        {
            public bool Equals(IGH_Param x, IGH_Param y)
            {
                return true;
                //throw new NotImplementedException();
            }

            public bool Equals1(IGH_Param x, IGH_Param y)
            {
                return x.InstanceGuid == y.InstanceGuid;
            }

            public int GetHashCode(IGH_Param obj)
            {
                return 0;
                //throw new NotImplementedException();
            }

            public int GetHashCode1(IGH_Param obj)
            {
                return obj.GetHashCode();
            }
        }

        public class GH_ClusterPasswordUndoAction : GH_ObjectUndoAction
        {
            private byte[] m_key;

            public GH_ClusterPasswordUndoAction(GH_Cluster cluster)
              : base(cluster.InstanceGuid)
            {
                this.m_key = cluster.m_password;
            }

            protected override void Object_Undo(GH_Document doc, IGH_DocumentObject obj)
            {
                GH_Cluster ghCluster = obj as GH_Cluster;
                if (ghCluster == null)
                    throw new InvalidCastException("Undo target is not a cluster");
                byte[] password = ghCluster.m_password;
                ghCluster.m_password = this.m_key;
                if (password == null)
                    this.m_key = (byte[])null;
                else
                    this.m_key = (byte[])password.Clone();
            }

            protected override void Object_Redo(GH_Document doc, IGH_DocumentObject obj)
            {
                this.Object_Undo(doc, obj);
            }
        }

        public class GH_ClusterPropertiesUndoAction : GH_ObjectUndoAction
        {
            private Bitmap m_icon;
            private string m_name;
            private string m_nickname;
            private string m_description;
            private IGH_Author m_author;

            public GH_ClusterPropertiesUndoAction(GH_Cluster cluster)
              : base(cluster.InstanceGuid)
            {
                this.m_name = cluster.Name;
                this.m_nickname = cluster.NickName;
                this.m_description = cluster.Description;
                this.m_author = (IGH_Author)new GH_Author(cluster.Author);
                //if (cluster.m_icon_override == null)
                //    return;
                this.m_icon = Icons._1;
            }

            protected override void Object_Undo(GH_Document doc, IGH_DocumentObject obj)
            {
                GH_Cluster ghCluster = obj as GH_Cluster;
                if (ghCluster == null)
                    throw new InvalidCastException("Undo target object is not a cluster");
                string name = obj.Name;
                string nickName = obj.NickName;
                string description = obj.Description;
                GH_Author ghAuthor = new GH_Author(ghCluster.Author);
                Bitmap bitmap = (Bitmap)null;
                //if (ghCluster.m_icon_override != null)
                //    bitmap = Icons._1;
                ghCluster.Name = this.m_name;
                ghCluster.NickName = this.m_nickname;
                ghCluster.Description = this.m_description;
                ghCluster.Author = this.m_author;
                ghCluster.SetIconOverride(this.m_icon);
                this.m_name = name;
                this.m_nickname = nickName;
                this.m_description = description;
                this.m_author = (IGH_Author)ghAuthor;
                this.m_icon = bitmap;
            }

            protected override void Object_Redo(GH_Document doc, IGH_DocumentObject obj)
            {
                this.Object_Undo(doc, obj);
            }
        }

        public class GH_ClusterPreviewDocumentAction : GH_ObjectUndoAction
        {
            private bool m_preview;

            public GH_ClusterPreviewDocumentAction(GH_Cluster cluster)
              : base(cluster.InstanceGuid)
            {
                this.m_preview = cluster.PreviewContent;
            }

            protected override void Object_Undo(GH_Document doc, IGH_DocumentObject obj)
            {
                GH_Cluster ghCluster = obj as GH_Cluster;
                if (ghCluster == null)
                    throw new InvalidCastException("Undo target is not a cluster");
                bool previewContent = ghCluster.PreviewContent;
                ghCluster.PreviewContent = this.m_preview;
                this.m_preview = previewContent;
            }

            protected override void Object_Redo(GH_Document doc, IGH_DocumentObject obj)
            {
                this.Object_Undo(doc, obj);
            }
        }

        public class GH_ClusterDocumentIdAction : GH_ObjectUndoAction
        {
            private Guid m_id;

            public GH_ClusterDocumentIdAction(GH_Cluster cluster)
              : base(cluster.InstanceGuid)
            {
                this.m_id = cluster.DocumentId;
            }

            protected override void Object_Undo(GH_Document doc, IGH_DocumentObject obj)
            {
                GH_Cluster ghCluster = obj as GH_Cluster;
                if (ghCluster == null)
                    throw new InvalidCastException("Undo target is not a cluster");
                Guid documentId = ghCluster.DocumentId;
                //ghCluster.m_internalDocument?.SetDocumentID(this.m_id);
                this.m_id = documentId;
            }

            protected override void Object_Redo(GH_Document doc, IGH_DocumentObject obj)
            {
                this.Object_Undo(doc, obj);
            }
        }

        public class GH_ClusterReferenceAction : GH_ObjectUndoAction
        {
            private string m_path;

            public GH_ClusterReferenceAction(GH_Cluster cluster)
              : base(cluster.InstanceGuid)
            {
                this.m_path = cluster.m_file.Path;
            }

            protected override void Object_Undo(GH_Document doc, IGH_DocumentObject obj)
            {
                GH_Cluster ghCluster = obj as GH_Cluster;
                if (ghCluster == null)
                    throw new InvalidCastException("Undo target is not a cluster");
                string path = ghCluster.m_file.Path;
                ghCluster.m_file.Path = this.m_path;
                this.m_path = path;
            }

            protected override void Object_Redo(GH_Document doc, IGH_DocumentObject obj)
            {
                this.Object_Undo(doc, obj);
            }
        }
    }
}