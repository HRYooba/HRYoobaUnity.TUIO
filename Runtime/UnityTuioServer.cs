using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TUIOsharp;
using TUIOsharp.DataProcessors;
using UniRx;

namespace HRYooba.Library.Network
{
    public class UnityTuioServer : IDisposable
    {
        private bool _disposed = false;
        private CompositeDisposable _disposables = new CompositeDisposable();

        private TuioServer _tuioServer = null;
        private BlobProcessor _blobProcessor = new BlobProcessor();
        private CursorProcessor _cursorProcessor = new CursorProcessor();
        private ObjectProcessor _objectProcessor = new ObjectProcessor();

        private Dictionary<int, TuioPointData> _points = new Dictionary<int, TuioPointData>();

        private Subject<TuioPointData> _onPointAdded = new Subject<TuioPointData>();
        private Subject<TuioPointData> _onPointUpdated = new Subject<TuioPointData>();
        private Subject<TuioPointData> _onPointRemoved = new Subject<TuioPointData>();

        public IReadOnlyList<TuioPointData> Points => _points.Values.ToList();
        public IObservable<TuioPointData> OnPointAdded => _onPointAdded.ObserveOnMainThread();
        public IObservable<TuioPointData> OnPointUpdated => _onPointUpdated.ObserveOnMainThread();
        public IObservable<TuioPointData> OnPointRemoved => _onPointRemoved.ObserveOnMainThread();

        public UnityTuioServer()
        {
            _blobProcessor.BlobAdded += OnBlobAdded;
            _blobProcessor.BlobUpdated += OnBlobUpdated;
            _blobProcessor.BlobRemoved += OnBlobRemoved;
            _cursorProcessor.CursorAdded += OnCursorAdded;
            _cursorProcessor.CursorUpdated += OnCursorUpdated;
            _cursorProcessor.CursorRemoved += OnCursorRemoved;
            _objectProcessor.ObjectAdded += OnObjectAdded;
            _objectProcessor.ObjectUpdated += OnObjectUpdated;
            _objectProcessor.ObjectRemoved += OnObjectRemoved;

            OnPointAdded.Subscribe(AddPointData).AddTo(_disposables);
            OnPointUpdated.Subscribe(UpdatePointData).AddTo(_disposables);
            OnPointRemoved.Subscribe(RemovePointData).AddTo(_disposables);
        }

        ~UnityTuioServer()
        {
            Dispose();
        }

        public void OpenServer(int port = 3333)
        {
            if (_tuioServer != null) return;

            _tuioServer = new TuioServer(port);
            _tuioServer.AddDataProcessor(_blobProcessor);
            _tuioServer.AddDataProcessor(_cursorProcessor);
            _tuioServer.AddDataProcessor(_objectProcessor);
            _tuioServer.Connect();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _disposables.Dispose();
            _disposables = null;

            if (_tuioServer != null)
            {
                _tuioServer.RemoveAllDataProcessors();
                _tuioServer.Disconnect();
                _tuioServer = null;
            }

            _blobProcessor.BlobAdded -= OnBlobAdded;
            _blobProcessor.BlobUpdated -= OnBlobUpdated;
            _blobProcessor.BlobRemoved -= OnBlobRemoved;
            _cursorProcessor.CursorAdded -= OnCursorAdded;
            _cursorProcessor.CursorUpdated -= OnCursorUpdated;
            _cursorProcessor.CursorRemoved -= OnCursorRemoved;
            _objectProcessor.ObjectAdded -= OnObjectAdded;
            _objectProcessor.ObjectUpdated -= OnObjectUpdated;
            _objectProcessor.ObjectRemoved -= OnObjectRemoved;

            _onPointAdded.Dispose();
            _onPointAdded = null;
            _onPointUpdated.Dispose();
            _onPointUpdated = null;
            _onPointRemoved.Dispose();
            _onPointRemoved = null;

            _points.Clear();
            _points = null;
        }

        private void AddPointData(TuioPointData pointData)
        {
            try
            {
                _points.Add(pointData.Id, pointData);
            }
            catch (ArgumentException e)
            {
                Debug.LogWarning($"[UnityTuioServer]: ID[{pointData.Id}] that already exists is about to be added.\n{e}");
            }
        }

        private void UpdatePointData(TuioPointData pointData)
        {
            _points[pointData.Id].UpdatePosition(pointData.Position);
        }

        private void RemovePointData(TuioPointData pointData)
        {
            _points.Remove(pointData.Id);
        }

        #region Blob
        private void OnBlobAdded(object sender, TuioBlobEventArgs e)
        {
            var entity = e.Blob;
            var pointData = new TuioPointData(TuioPointType.Blob, entity.Id, new Vector2(entity.X, 1.0f - entity.Y));
            _onPointAdded.OnNext(pointData);
        }

        private void OnBlobUpdated(object sender, TuioBlobEventArgs e)
        {
            var entity = e.Blob;
            var pointData = new TuioPointData(TuioPointType.Blob, entity.Id, new Vector2(entity.X, 1.0f - entity.Y));
            _onPointUpdated.OnNext(pointData);
        }

        private void OnBlobRemoved(object sender, TuioBlobEventArgs e)
        {
            var entity = e.Blob;
            var pointData = new TuioPointData(TuioPointType.Blob, entity.Id, new Vector2(entity.X, 1.0f - entity.Y));
            _onPointRemoved.OnNext(pointData);
        }
        #endregion

        #region Cursor
        private void OnCursorAdded(object sender, TuioCursorEventArgs e)
        {
            var entity = e.Cursor;
            var pointData = new TuioPointData(TuioPointType.Cursor, entity.Id, new Vector2(entity.X, 1.0f - entity.Y));
            _onPointAdded.OnNext(pointData);
        }

        private void OnCursorUpdated(object sender, TuioCursorEventArgs e)
        {
            var entity = e.Cursor;
            var pointData = new TuioPointData(TuioPointType.Cursor, entity.Id, new Vector2(entity.X, 1.0f - entity.Y));
            _onPointUpdated.OnNext(pointData);
        }

        private void OnCursorRemoved(object sender, TuioCursorEventArgs e)
        {
            var entity = e.Cursor;
            var pointData = new TuioPointData(TuioPointType.Cursor, entity.Id, new Vector2(entity.X, 1.0f - entity.Y));
            _onPointRemoved.OnNext(pointData);
        }
        #endregion

        #region Object
        private void OnObjectAdded(object sender, TuioObjectEventArgs e)
        {
            var entity = e.Object;
            var pointData = new TuioPointData(TuioPointType.Object, entity.Id, new Vector2(entity.X, 1.0f - entity.Y));
            _onPointAdded.OnNext(pointData);
        }

        private void OnObjectUpdated(object sender, TuioObjectEventArgs e)
        {
            var entity = e.Object;
            var pointData = new TuioPointData(TuioPointType.Object, entity.Id, new Vector2(entity.X, 1.0f - entity.Y));
            _onPointUpdated.OnNext(pointData);
        }

        private void OnObjectRemoved(object sender, TuioObjectEventArgs e)
        {
            var entity = e.Object;
            var pointData = new TuioPointData(TuioPointType.Object, entity.Id, new Vector2(entity.X, 1.0f - entity.Y));
            _onPointRemoved.OnNext(pointData);
        }
        #endregion
    }
}

