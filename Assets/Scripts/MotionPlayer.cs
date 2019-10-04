using UnityEngine;
using System.IO;
using sample_app.single.ios.Main;
#if UNITY_EDITOR
using UnityEditor;
#endif

// TODO : 最終的に使用しないのであれば削除
public class MotionPlayer : MonoBehaviour
{
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

    private void Awake()
    {
        _animation = _target.GetComponent<Animation>();
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

            var clip = _animation.GetClip($"ImportClip");
            if (clip != null)
            {
                _animation?.Play(clip.name);
                _count++;
                if (_count >= _animation.GetClipCount())
                {
                    _count = 0;
                }
            }
        }
        // アニメーションクリップ作成.
        else if (Input.GetKeyDown(KeyCode.L))
        {
            if (_csv != null)
            {
                var clip = CSVMotionDataImporter.Import(_csv.text, _target);
                _target.GetComponent<Animation>().AddClip(clip, $"ImportClip");

#if UNITY_EDITOR
                Directory.CreateDirectory($"Assets/Animations/");
                AssetDatabase.CreateAsset(clip, $"Assets/Animations/Clip.anim");
#endif
            }
        }
    }
}

