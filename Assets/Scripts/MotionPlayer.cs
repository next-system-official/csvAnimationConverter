using UnityEngine;
using System.IO;
using sample_app.single.ios.Main;
#if UNITY_EDITOR
using UnityEditor;
#endif

// TODO : 最終的に使用しないのであれば削除
public class MotionPlayer : MonoBehaviour
{
    private readonly string CLIP_NAME = "ImportClip";
    /// <summary>
    /// モーションを再生するオブジェクト．
    /// </summary>
    [SerializeField]
    private Transform _target;
    /// <summary>
    /// 書き出すCSVファイルの番号（保存順の上から番号０～）．
    /// </summary>
    private int _count = 0;
    [SerializeField]
    private Animation _animation = null;
    [SerializeField]
    private TextAsset _csv = null;
    private AnimationClip _clip = null;
    private void Awake()
    {
        _animation = _target.GetComponent<Animation>();
        if (_animation == null)
        {
            _animation = _target.gameObject.AddComponent<Animation>();
        }
    }

    private void Update()
    {

        // 作成したアニメーションクリップ再生.
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (_animation == null)
            {
                return;
            }
            if (_animation?.GetClipCount() == null)
            {
                return;
            }
            _animation?.Play(CLIP_NAME);
        }
        // アニメーションクリップ作成.
        else if (Input.GetKeyDown(KeyCode.L))
        {
            if (_csv != null)
            {
                var clip = CSVMotionDataImporter.Import(_csv.text, _target);
                clip.legacy = true;
                _animation?.AddClip(clip, CLIP_NAME);
#if UNITY_EDITOR
                Directory.CreateDirectory($"Assets/Animations/");
                AssetDatabase.CreateAsset(clip, $"Assets/Animations/Clip.anim");
#endif
            }
        }
    }
}

