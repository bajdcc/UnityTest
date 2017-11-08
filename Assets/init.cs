using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class init : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	}

	//方向灵敏度
	public float sensitivityKeyboardX = 0.1F;
	public float sensitivityKeyboardY = 0.1F;

	//方向灵敏度
	public float sensitivityX = 10F;
	public float sensitivityY = 10F;

	//上下最大视角(Y视角)
	public float minimumY = -60F;
	public float maximumY = 60F;

	private Vector3 PreMouseMPos;
	private Vector3 PreMouseLPos;

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}

		CameraMove ();
		MouseScrollWheel ();
		Move ();
	}

	private void MouseScrollWheel ()
	{
		//滚轮前进后退
		if (Input.GetAxis ("Mouse ScrollWheel") != 0) {
			gameObject.transform.Translate (new Vector3 (0, 0, Input.GetAxis ("Mouse ScrollWheel") * Time.deltaTime * 500));
		}
	}

	private void CameraMove ()
	{
		//鼠标中键按下，上下左右移动相机
		if (Input.GetMouseButton (2)) {
			if (PreMouseMPos.x <= 0) {
				PreMouseMPos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0.0f);
			} else {
				Vector3 CurMouseMPos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0.0f);
				Vector3 offset = CurMouseMPos - PreMouseMPos;
				offset = -offset * 0.1f;//0.1这个数字的大小可以调节速度
				transform.Translate (offset);
				PreMouseMPos = CurMouseMPos;
			}
		} else {
			if (Input.GetAxis("Horizontal") != 0)
			{
				transform.Translate(Input.GetAxis("Horizontal") * sensitivityKeyboardX, 0, 0);
			}
			if (Input.GetAxis("Vertical") != 0)
			{
				transform.Translate(0, Input.GetAxis("Vertical") * sensitivityKeyboardY, 0);
			}
			PreMouseMPos = new Vector3 (0.0f, 0.0f, 0.0f);
		}
	}

	private float m_TurnSpeed = 8f;                    // How fast the rig will rotate from user input.
	private float m_LookAngleX = 0f;                   // The rig's x axis rotation.
	private float m_LookAngleY = 0f;                   // The rig's y axis rotation.

	private void Move ()
	{
		if(Time.timeScale < float.Epsilon)
			return;

		if (!Input.GetMouseButton (1))
			return;
		
		// Read the user input
		var x = Input.GetAxis("Mouse X");
		var y = Input.GetAxis("Mouse Y");

		// Adjust the look angle by an amount proportional to the turn speed and horizontal input.
		m_LookAngleX -= y*m_TurnSpeed;
		m_LookAngleY += x*m_TurnSpeed;

		// Rotate the rig (the root object) around Y axis only:
		transform.localRotation = Quaternion.Euler(m_LookAngleX, m_LookAngleY, 0f);
	}
}
