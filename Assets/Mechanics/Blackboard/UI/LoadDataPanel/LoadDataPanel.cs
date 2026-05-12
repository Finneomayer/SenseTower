using UI;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using Assets.Scripts.Space;

namespace Assets.Blackboard
{

    public sealed class LoadDataPanel : ViewPanel
    {
        [SerializeField]
        private BlackboardSnapshotsView BlackboardSnapshotsView;

        private BlackBoard _blackboard;
        private BlackboardEventMediator _blackboardEventMediator = new();
        private BlackboardDataSaver _blackboardDataSaver = new();

        private Dictionary<string, BlackboardSnapshotData> _loadedFiles = new();

        private ISpaceManager _spaceManager;

        private void OnEnable()
        {
            _blackboardEventMediator.LoadBlackboardFileRequested += OnLoadBlackboardFileRequested;
            _blackboardEventMediator.DeleteBlackboardFileRequested += OnDeleteBlackboardFileRequested;
        }

        private void OnDisable()
        {
            _blackboardEventMediator.LoadBlackboardFileRequested -= OnLoadBlackboardFileRequested;
            _blackboardEventMediator.DeleteBlackboardFileRequested -= OnDeleteBlackboardFileRequested;
        }

        public void Init(BlackBoard blackboard, ISpaceManager spaceManager)
        {
            _blackboard = blackboard;
            _spaceManager = spaceManager;
            BlackboardSnapshotsView.Init(_blackboardEventMediator);
        }

        public override void ShowPanel()
        {            
            base.ShowPanel();
            BlackboardSnapshotsView.DestroyItems();
            _loadedFiles.Clear();

            LoadSnapshotsDataAsync().Forget();
        }

        public override void HidePanel()
        {
            base.HidePanel();

            _blackboardDataSaver.InterruptLoading();
            BlackboardSnapshotsView.DestroyItems();
            _loadedFiles.Clear();
        }

        private void OnLoadBlackboardFileRequested(string filename)
        {
            if (_loadedFiles.TryGetValue(filename, out BlackboardSnapshotData data))
            {
                _blackboard.SetData(data.Data);
            }
        }

        private void OnDeleteBlackboardFileRequested(string filename)
        {
            BlackboardSnapshotsView.RemoveItem(filename);
            _loadedFiles.Remove(filename);
            BlackboardDataSaver.RemoveFile(filename);
        }

        private void OnSnapshotFileLoaded(string filename, BlackboardSnapshotData data)
        {
            _loadedFiles[filename] = data;
            BlackboardSnapshotsView.AddItem(filename, data.BlackboardScreenThumbnailData);
        }

        private async UniTask LoadSnapshotsDataAsync()
        {
            if (_spaceManager == null || _spaceManager.CurrentTransitionTarget == null)
            {
                return;
            }

            while (_blackboardDataSaver.LoadingInProgress)
            {
                await UniTask.DelayFrame(1);
                if (!IsVisible())
                {
                    return;
                }
            }

            _blackboardDataSaver.LoadFiles(_spaceManager.CurrentTransitionTarget.Id, _blackboard.BlackboardId, 
                OnSnapshotFileLoaded);
        }
    }
}


