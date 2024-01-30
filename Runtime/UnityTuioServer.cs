using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TUIOsharp;
using TUIOsharp.DataProcessors;
using UniRx;

namespace HRYooba.TUIO
{
    public class UnityTuioServer : IDisposable
    {
        private bool _disposed = false;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly Dictionary<int, TuioPointData> _points = new Dictionary<int, TuioPointData>();

        // TUIO
        private TuioServer _tuioServer = null;
        private readonly BlobProcessor _blobProcessor = new BlobProcessor();
        private readonly CursorProcessor _cursorProcessor = new CursorProcessor();
        private readonly ObjectProcessor _objectProcessor = new ObjectProcessor();

        private readonly Subject<TuioPointData> _onPointAddedSubject = new Subject<TuioPointData>();
        private readonly Subject<TuioPointData> _onPointUpdatedSubject = new Subject<TuioPointData>();
        private readonly Subject<TuioPointData> _onPointRemovedSubject = new Subject<TuioPointData>();

        public IReadOnlyList<TuioPointData> Points => _points.Values.ToList();
        public IObservable<TuioPointData> OnPointAddedObservable => _onPointAddedSubject.ObserveOnMainThread();
        public IObservable<TuioPointData> OnPointUpdatedObservable => _onPointUpdatedSubject.ObserveOnMainThread();
        public IObservable<TuioPointData> OnPointRemovedObservable => _onPointRemovedSubject.ObserveOnMainThread();

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

            OnPointAddedObservable.Subscribe(AddPointData).AddTo(_disposables);
            OnPointUpdatedObservable.Subscribe(UpdatePointData).AddTo(_disposables);
            OnPointRemovedObservable.Subscribe(RemovePointData).AddTo(_disposables);
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

            _onPointAddedSubject.Dispose();
            _onPointUpdatedSubject.Dispose();
            _onPointRemovedSubject.Dispose();

            _points.Clear();
        }

        private void AddPointData(TuioPointData pointData)
        {
            _points.TryAdd(pointData.Id, pointData);
        }

        private void UpdatePointData(TuioPointData pointData)
        {
            if (_points.ContainsKey(pointData.Id))
            {
                _points[pointData.Id].SetPosition(pointData.Position);
            }
        }

        private void RemovePointData(TuioPointData pointData)
        {
            if (_points.ContainsKey(pointData.Id))
            {
                _points.Remove(pointData.Id);
            }
        }

        #region Blob
        private void OnBlobAdded(object sender, TuioBlobEventArgs e)
        {
            var entity = e.Blob;
            var pointData = new TuioPointData(TuioPointType.Blob, entity.Id, new Vector2(entity.X, 1.0f - entity.Y));
            _onPointAddedSubject.OnNext(pointData);
        }

        private void OnBlobUpdated(object sender, TuioBlobEventArgs e)
        {
            var entity = e.Blob;
            var pointData = new TuioPointData(TuioPointType.Blob, entity.Id, new Vector2(entity.X, 1.0f - entity.Y));
            _onPointUpdatedSubject.OnNext(pointData);
        }

        private void OnBlobRemoved(object sender, TuioBlobEventArgs e)
        {
            var entity = e.Blob;
            var pointData = new TuioPointData(TuioPointType.Blob, entity.Id, new Vector2(entity.X, 1.0f - entity.Y));
            _onPointRemovedSubject.OnNext(pointData);
        }
        #endregion

        #region Cursor
        private void OnCursorAdded(object sender, TuioCursorEventArgs e)
        {
            var entity = e.Cursor;
            var pointData = new TuioPointData(TuioPointType.Cursor, entity.Id, new Vector2(entity.X, 1.0f - entity.Y));
            _onPointAddedSubject.OnNext(pointData);
        }

        private void OnCursorUpdated(object sender, TuioCursorEventArgs e)
        {
            var entity = e.Cursor;
            var pointData = new TuioPointData(TuioPointType.Cursor, entity.Id, new Vector2(entity.X, 1.0f - entity.Y));
            _onPointUpdatedSubject.OnNext(pointData);
        }

        private void OnCursorRemoved(object sender, TuioCursorEventArgs e)
        {
            var entity = e.Cursor;
            var pointData = new TuioPointData(TuioPointType.Cursor, entity.Id, new Vector2(entity.X, 1.0f - entity.Y));
            _onPointRemovedSubject.OnNext(pointData);
        }
        #endregion

        #region Object
        private void OnObjectAdded(object sender, TuioObjectEventArgs e)
        {
            var entity = e.Object;
            var pointData = new TuioPointData(TuioPointType.Object, entity.Id, new Vector2(entity.X, 1.0f - entity.Y));
            _onPointAddedSubject.OnNext(pointData);
        }

        private void OnObjectUpdated(object sender, TuioObjectEventArgs e)
        {
            var entity = e.Object;
            var pointData = new TuioPointData(TuioPointType.Object, entity.Id, new Vector2(entity.X, 1.0f - entity.Y));
            _onPointUpdatedSubject.OnNext(pointData);
        }

        private void OnObjectRemoved(object sender, TuioObjectEventArgs e)
        {
            var entity = e.Object;
            var pointData = new TuioPointData(TuioPointType.Object, entity.Id, new Vector2(entity.X, 1.0f - entity.Y));
            _onPointRemovedSubject.OnNext(pointData);
        }
        #endregion
    }
}

