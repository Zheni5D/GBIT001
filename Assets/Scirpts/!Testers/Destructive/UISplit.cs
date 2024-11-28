using System.Collections;
using System.Collections.Generic;
using Chronos;
using DG.Tweening;
using UnityEngine;

public class UISplit : MonoBehaviour
{
	[Range(1, 20)] public int radiateNum = 5; //裂痕的数量
	[Range(0, 45)] public int changeAngle = 15; //随机裂痕的弯曲程度
	[Tooltip("每隔多少像素再转一次弯")] public int pixelL;
	[Header("物理模式")] public float force;
	public float drag;

	[Header("运动学模式")] [Tooltip("质量越小，飞得越远。这个是mass=1的飞的距离。其他质量线性变换")]
	public float distance;

	private Color[] color;
	private int w, h;
	private Vector3 receiveForcePoint;
	public Transform testTrans;
	[SerializeField] private Transform generateTrans;
	[SerializeField] [Range(0, 1.0f)] private float offset;

	[Tooltip("假想受击点，用于设置碎片移动方向")] [SerializeField]
	private float HyptheticalPointDist;

	private Vector3 transUp;
	private bool flag;

	private void Start()
	{
		if (flag) return;
		Texture2D texture = GetComponent<SpriteRenderer>().sprite.texture;
		if (!texture.isReadable)
		{
			Debug.LogError("图片不可读！请勾选图片属性中的Read/Write Enabled选项.");
			return;
		}

		color = texture.GetPixels();
		w = texture.width;
		h = texture.height;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			if (testTrans != null)
				Trigger(testTrans.position);
		}
	}

	public void TriggerInit()
	{
		flag = true;
		Texture2D texture = GetComponent<SpriteRenderer>().sprite.texture;
		if (!texture.isReadable)
		{
			Debug.LogError("图片不可读！请勾选图片属性中的Read/Write Enabled选项.");
			return;
		}

		color = texture.GetPixels();
		w = texture.width;
		h = texture.height;
	}

	// public Vector3 tmurderPoint;
	// public void LOG(){
	// 	var dir = generateTrans.position - tmurderPoint;
	// 	Debug.Log("dir="+dir);
	// 	Debug.Log(Vector3.Dot(dir,transUp));
	// }

	public void Trigger(Vector3 murderPoint, bool isDoor = true)
	{
// tmurderPoint = murderPoint;

		//门的上边是右边
		if (isDoor)
			transUp = transform.right;
		else
			transUp = transform.up;
		//murderPoint可能不在被破碎物体的Collider区域内
		Vector3 dir = generateTrans.position * 10 - murderPoint * 10; //防止过小舍入
		dir.z = 0;
		float dot = Vector3.Dot(dir, transUp);

		if (dot <= 0)
		{
			//为什么是<=不是>=我也想知道
			offset = Mathf.Abs(offset);
		}
		else
		{
			offset = -Mathf.Abs(offset);
		}


		receiveForcePoint = generateTrans.position + transUp.normalized * offset;
		Vector3 clickPos = new Vector3(w, h) * 0.5f;
		Vector3 pos1 = (transUp.normalized * offset) * 100;
		pos1.x /= transform.lossyScale.x;
		pos1.y /= transform.lossyScale.y;
		Vector3 pos2 = new Vector3();
		float angle = -transform.eulerAngles.z * Mathf.Deg2Rad;
		pos2.x = pos1.x * Mathf.Cos(angle) - pos1.y * Mathf.Sin(angle);
		pos2.y = pos1.x * Mathf.Sin(angle) + pos1.y * Mathf.Cos(angle);
		clickPos += pos2;
		CreateSprites((int)clickPos.x, (int)clickPos.y);
	}

	// private void OnMouseDown() {
	// 	Vector3 clickPos = new Vector3(w, h) * 0.5f;
	// 	Vector3 pos1 = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position) * 100;
	// 	pos1.x /= transform.lossyScale.x;
	// 	pos1.y /= transform.lossyScale.y;
	// 	Vector3 pos2 = new Vector3();
	// 	float angle = -transform.eulerAngles.z * Mathf.Deg2Rad;
	// 	pos2.x = pos1.x * Mathf.Cos(angle) - pos1.y * Mathf.Sin(angle);
	// 	pos2.y = pos1.x * Mathf.Sin(angle) + pos1.y * Mathf.Cos(angle);
	// 	clickPos += pos2;
	// 	CreateSprites((int)clickPos.x, (int)clickPos.y);
	// }

	//创建分裂图
	private void CreateSprites(int center_j, int center_i)
	{
		//随机分裂角度
		float[] angle = new float[radiateNum];
		int num = 0;
		bool retry; //随机角度接近时要重新生成
		float delt;
		while (num != radiateNum)
		{
			angle[num] = Random.Range(0, 2 * Mathf.PI);
			retry = false;
			for (int i = 0; i < num; i++)
			{
				delt = Mathf.Abs(angle[i] - angle[num]);
				if (delt < Mathf.PI / radiateNum / 2 || delt > 2 * Mathf.PI - Mathf.PI / radiateNum / 2)
				{
					retry = true;
					break;
				}
			}

			if (!retry)
			{
				num++;
			}
		}

		//绘制随机裂痕
		bool[] line = new bool[w * h];
		line[center_i * w + center_j] = true;
		for (int i = 0; i < radiateNum; i++)
		{
			Vector2 curVec = new Vector2(center_i, center_j);
			Pos curPos;
			Vector2 step = new Vector2(Mathf.Cos(angle[i]), Mathf.Sin(angle[i])) * 0.98f;
			int stepNum = 1;
			while (true)
			{
				if (stepNum % pixelL == 0)
				{
					angle[i] += Random.Range(-changeAngle * Mathf.Deg2Rad, changeAngle * Mathf.Deg2Rad);
					step = new Vector2(Mathf.Cos(angle[i]), Mathf.Sin(angle[i])) * 0.98f;
				}

				curVec += step;
				curPos = new Pos(curVec.x, curVec.y);
				if (curPos.i >= 0 && curPos.j >= 0 && curPos.i < h && curPos.j < w)
				{
					line[curPos.i * w + curPos.j] = true;
				}
				else
				{
					break;
				}

				stepNum++;
			}
		}

		//向四周扩展从而获取每块图像
		GameObject g1 = new GameObject("sprites");
		g1.transform.position = generateTrans.position;
		g1.AddComponent<StageReverseObject>().setStageID(((StageID)LevelObject.curStageIndex));
		Timeline timeline = g1.AddComponent<Timeline>();
		timeline.mode = TimelineMode.Global;
		timeline.rewindable = false;
		timeline.globalClockKey = Timekeeper.instance.Clock("VFX").key;
		int splitNum = 0;
		for (int n = 0; n < w * h; n++)
		{
			if (!line[n])
			{
				splitNum++;
				List<Pos> list = new List<Pos>();
				List<Pos> list1 = new List<Pos>();
				list.Add(new Pos(n / w, n % w));
				list1.Add(new Pos(n / w, n % w));
				line[n] = true;
				int left, right, down, up;
				left = right = n % w;
				down = up = n / w;
				while (list.Count != 0)
				{
					Pos p = list[0];
					list.RemoveAt(0);
					if (p.i > 0 && !line[(p.i - 1) * w + p.j])
					{
						list.Add(new Pos(p.i - 1, p.j));
						list1.Add(new Pos(p.i - 1, p.j));
						line[(p.i - 1) * w + p.j] = true;
						if (p.i - 1 < down)
						{
							down = p.i - 1;
						}
					}

					if (p.j > 0 && !line[p.i * w + p.j - 1])
					{
						list.Add(new Pos(p.i, p.j - 1));
						list1.Add(new Pos(p.i, p.j - 1));
						line[p.i * w + p.j - 1] = true;
						if (p.j - 1 < left)
						{
							left = p.j - 1;
						}
					}

					if (p.i < h - 1 && !line[(p.i + 1) * w + p.j])
					{
						list.Add(new Pos(p.i + 1, p.j));
						list1.Add(new Pos(p.i + 1, p.j));
						line[(p.i + 1) * w + p.j] = true;
						if (p.i + 1 > up)
						{
							up = p.i + 1;
						}
					}

					if (p.j < w - 1 && !line[p.i * w + p.j + 1])
					{
						list.Add(new Pos(p.i, p.j + 1));
						list1.Add(new Pos(p.i, p.j + 1));
						line[p.i * w + p.j + 1] = true;
						if (p.j + 1 > right)
						{
							right = p.j + 1;
						}
					}
				}

				//创建sprite
				// PhysicsMaterial2D m;
				// m = new PhysicsMaterial2D("m");
				// m.bounciness = 0.3f;    //物体边缘的弹性和摩擦系数
				// m.friction = 0.6f;
				int w1 = right - left + 1;
				int h1 = up - down + 1;
				int pos_x = (left + right - w) / 2;
				int pos_y = (down + up - h) / 2;
				Vector3 pos1 = new Vector3(pos_x, pos_y, 0) / 100;
				Vector3 pos2 = new Vector3();
				float angle1 = transform.eulerAngles.z * Mathf.Deg2Rad;
				pos1.x *= transform.lossyScale.x;
				pos1.y *= transform.lossyScale.y;
				pos2.x = pos1.x * Mathf.Cos(angle1) - pos1.y * Mathf.Sin(angle1);
				pos2.y = pos1.x * Mathf.Sin(angle1) + pos1.y * Mathf.Cos(angle1);
				if (list1.Count > 20)
				{
					//大于20像素才会创建
					Texture2D t1 = new Texture2D(w1, h1);
					Color[] c1 = new Color[w1 * h1];
					foreach (Pos p in list1)
					{
						if (p.i == down || p.i == up || p.j == left || p.j == right)
						{
							c1[(p.i - down) * w1 + p.j - left] = Color.clear;
						}
						else
							c1[(p.i - down) * w1 + p.j - left] = color[p.i * w + p.j];
					}

					t1.SetPixels(c1);
					t1.Apply();
					GameObject g = new GameObject("sprite");
					g.transform.SetParent(g1.transform);
					g.transform.position = generateTrans.position + pos2;
					g.transform.eulerAngles = new Vector3(0, 0, angle1 * Mathf.Rad2Deg);
					g.transform.localScale = transform.lossyScale;
					g.AddComponent<SpriteRenderer>().sprite =
						Sprite.Create(t1, new Rect(0, 0, w1, h1), new Vector2(0.5f, 0.5f));
					g.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
					g.GetComponent<SpriteRenderer>().sortingOrder = 111;
					// g.AddComponent<PolygonCollider2D>().sharedMaterial = m;
					//Rigidbody2D spriteRigidbody = g.AddComponent<Rigidbody2D>();
					float mass;
					if (GetComponent<Rigidbody2D>())
					{
						//spriteRigidbody.velocity = GetComponent<Rigidbody2D>().velocity;
						mass = list1.Count / (float)(w * h) * GetComponent<Rigidbody2D>().mass;
					}
					else
					{
						mass = list1.Count / (float)(w * h) * 20; //重新分配每块的质量
					}

					Vector3 forceDir = g.transform.position -
					                   (receiveForcePoint + transUp.normalized * (offset * HyptheticalPointDist));
					forceDir.z = 0;

					mass = GetComponent<Rigidbody2D>().mass - mass;
					mass *= mass;

					//TimeScaledDoMove.Instance.DoMove(timelineChild,g.transform.position+forceDir.normalized * distance * mass,1f);
					Tween tween = g.transform.DOMove(g.transform.position + forceDir.normalized * (distance * mass), 2f)
						.SetEase(Ease.OutQuart);
					tween.SetAutoKill(true);
					//g.transform.DOLocalRotate(new Vector3(0,0,Random.Range(10.0f,120.0f)),Random.Range(.5f,2.0f)).SetEase(Ease.OutQuart).SetAutoKill(true);
					//spriteRigidbody.AddForce(forceDir.normalized * force);
					//g.AddComponent<Split>().radiateNum = radiateNum;
				}
			}
		}
		//销毁原物体
		// gameObject.SetActive(false);
	}
}
