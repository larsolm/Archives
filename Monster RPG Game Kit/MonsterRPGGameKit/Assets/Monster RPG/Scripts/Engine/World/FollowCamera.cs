using UnityEngine;

namespace PiRhoSoft.MonsterRpgEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	[HelpURL(MonsterRpg.DocumentationUrl + "follow-camera")]
	[AddComponentMenu("PiRho Soft/World/Follow Camera")]
	public class FollowCamera : MonoBehaviour
	{
		[Tooltip("The speed at which to move the camera when following an object")]
		[Range(0.0f, 100.0f)]
		public float MoveSpeed = 10.0f;

		[Tooltip("The speed at which to zoom the camera when zooming in or out")]
		[Range(0.0f, 100.0f)]
		public float ZoomSpeed = 1.0f;

		[Tooltip("The object that this camera should begin following")]
		public GameObject TargetObject;

		private Camera _camera;

		private const float _defaultDistance = -10.0f;
		private float _defaultSize;
		private Vector2 _targetPosition;
		private float _targetSize;

		private bool _warpPosition = true;
		private bool _warpSize = true;
		private bool _hasBounds = false;
		private Rect _bounds = new Rect();

		protected virtual void Awake()
		{
			_camera = GetComponent<Camera>();
			_defaultSize = _camera.orthographicSize;
		}

		protected virtual void Update()
		{
			if (TargetObject)
				_warpPosition = _warpPosition || TargetObject.GetComponent<Mover>().DidWarp;
		}

		protected virtual void LateUpdate()
		{
			if (TargetObject)
			{
				_warpPosition = _warpPosition || TargetObject.GetComponent<Mover>().DidWarp;
				_targetPosition = TargetObject.transform.position;
			}

			var moveSpeed = Time.fixedDeltaTime * MoveSpeed;
			var zoomSpeed = Time.fixedDeltaTime * ZoomSpeed;
			var position = _warpPosition ? _targetPosition : Vector2.MoveTowards(transform.position, _targetPosition, moveSpeed);
			var size = _warpSize ? _targetSize : Mathf.MoveTowards(_camera.orthographicSize, _targetSize, zoomSpeed);
			var targetPosition = ClampBounds(position, size);

			_warpPosition = false;
			_warpSize = false;

			transform.SetPositionAndRotation(new Vector3(targetPosition.x, targetPosition.y, _defaultDistance), Quaternion.identity);
		}

		public void SetBounds(Rect bounds)
		{
			_hasBounds = true;
			_bounds = bounds;
		}

		public void ClearBounds()
		{
			_hasBounds = false;
		}

		public void StartFollowing(GameObject gameObject, bool warp)
		{
			TargetObject = gameObject;
			_warpPosition = warp;
		}

		public void StopFollowing()
		{
			TargetObject = null;
		}

		public void WarpToZoom(float zoom)
		{
			_warpSize = true;
			_targetSize = zoom > 0.0f ? _defaultSize * (1 / zoom) : _defaultSize;
		}

		public void MoveTowardZoom(float zoom)
		{
			_targetSize = zoom > 0.0f ? _defaultSize * (1 / zoom) : _defaultSize;
		}

		public void WarpToPosition(Vector2 position)
		{
			_warpPosition = true;
			_targetPosition = position;
		}

		public void MoveTowardsPosition(Vector2 position)
		{
			_targetPosition = position;
		}

		protected Vector2 ClampBounds(Vector2 position, float size)
		{
			if (_hasBounds)
				return ClampToBounds(position, size, _bounds, true, true, true, true);
			else if (Player.Instance.Zone != null && Player.Instance.Zone.Properties != null && Player.Instance.Zone.Properties.ClampBounds)
				return ClampToZone(position, size, Player.Instance.Zone.Properties);
			else
				return position;
		}

		protected Vector2 ClampToBounds(Vector2 position, float size, Rect clampArea, bool left, bool right, bool bottom, bool top)
		{
			var halfHeight = _camera.orthographicSize;
			var halfWidth = _camera.aspect * halfHeight;
			var height = halfHeight * 2.0f;
			var width = halfWidth * 2.0f;

			if (width > clampArea.width)
			{
				clampArea.x = clampArea.center.x - halfWidth;
				clampArea.width = width;
			}

			if (height > clampArea.height)
			{
				clampArea.y = clampArea.center.y - halfHeight;
				clampArea.height = height;
			}

			var bounds = new Rect(new Vector2(position.x - halfWidth, position.y - halfHeight), new Vector2(width, height));

			if (left && bounds.xMin < clampArea.xMin)
				position.x = clampArea.xMin + halfWidth;
			else if (right && bounds.xMax > clampArea.xMax)
				position.x = clampArea.xMax - halfWidth;

			if (bottom && bounds.yMin < clampArea.yMin)
				position.y = clampArea.yMin + halfHeight;
			else if (top && bounds.yMax > clampArea.yMax)
				position.y = clampArea.yMax - halfHeight;

			return position;
		}

		protected Vector2 ClampToZone(Vector2 position, float size, MapProperties map)
		{
			var bounds = new Rect(map.LeftBounds, map.BottomBounds, map.RightBounds - map.LeftBounds, map.TopBounds - map.BottomBounds);
			return ClampToBounds(position, size, bounds, map.ClampLeftBounds, map.ClampRightBounds, map.ClampBottomBounds, map.ClampTopBounds);
		}
	}
}
