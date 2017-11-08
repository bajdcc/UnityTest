using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createCubes : MonoBehaviour {

	private float m_Padding = 0.4f;
	private float m_Size = 1f;

	// Use this for initialization
	void Start () {
		var arr = new int[,]{
			{ 5, 6, 1, 4, 3, 2 },
			{ 4, 1, 3, 2, 6, 5 },
			{ 2, 3, 6, 1, 5, 4 },
			{ 6, 5, 4, 3, 2, 1 },
			{ 1, 2, 5, 6, 4, 3 },
			{ 3, 4, 2, 5, 1, 6 }
		};

		for (int i = 0; i < 6; i++) {
			for (int j = 0; j < 6; j++) {
				var cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
				cube.transform.localScale = new Vector3 (1, arr[i,j], 1);
				cube.transform.position = new Vector3 (i * (m_Size + m_Padding), arr[i,j]/2.0f, j * (m_Size + m_Padding));
				var material = new Material(Shader.Find("Legacy Shaders/Diffuse")); // 注意坑，不是所有shader默认都打包的，在Graphics Settings里面
				material.color = new Color (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f));
				cube.GetComponent<MeshRenderer> ().material = material;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
