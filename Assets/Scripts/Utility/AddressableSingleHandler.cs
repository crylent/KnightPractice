using System.Collections;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Utility
{
    public class AddressableSingleHandler<T> where T: Object
    {
        private readonly MonoBehaviour _owner;
        private readonly AsyncOperationHandle _handle;
        private T _result;
        [CanBeNull] private T _instance;
        public bool HasInstance => !_instance.IsUnityNull() && !_instance.IsDestroyed();

        public AddressableSingleHandler(MonoBehaviour owner, string addressableName)
        {
            _owner = owner;
            _handle = Addressables.LoadAssetAsync<GameObject>(addressableName);
            _handle.Completed += handle => _result = ((GameObject) handle.Result).GetComponent<T>();
            AddressablesController.Instance.StartCoroutine(ReleaseOnDestroy());
        }

        public T Instantiate()
        {
            _instance = Object.Instantiate(_result, _owner.transform);
            return _instance;
        }

        public T PopInstance()
        {
            var instance = _instance;
            _instance = null;
            return instance;
        }

        private IEnumerator ReleaseOnDestroy()
        {
            yield return new WaitUntil(_owner.IsDestroyed);
            Addressables.Release(_handle);
        }
    }
}