using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Utility
{
    public class AddressableStaticHandler<T> where T: Object
    {
        private readonly AsyncOperationHandle _handle;
        private T _result;
        private Transform _transform;

        public AddressableStaticHandler(string addressableName)
        {
            _handle = Addressables.LoadAssetAsync<GameObject>(addressableName);
            _handle.Completed += OnHandleCompleted;
        }

        private void OnHandleCompleted(AsyncOperationHandle handle)
        {
            var result = (GameObject) handle.Result;
            _result = ((GameObject) handle.Result).GetComponent<T>();
            _transform = result.transform;
        }

        public T Instantiate(Transform transform)
        {
            var instance = Object.Instantiate(_result, transform.position + _transform.position, Quaternion.identity);
            return instance;
        }

        public void Release()
        {
            Addressables.Release(_handle);
        }
    }
}