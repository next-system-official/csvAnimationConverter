using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace sample_app.single.ios.Main
{
    public class CSVMotionDataImporter
    {
        /// <summary>
        /// アバターパーツが持つアニメーションカーブの回転情報のみ要素数.
        /// </summary> 
        private static readonly int CURVE_NUM = 4;
        /// <summary>
        /// アバターパーツが持つアニメーションカーブの回転＋座標情報要素数.
        /// </summary> 
        private static readonly int CURVE_NUM_HIPS = 7;
        /// <summary>
        /// 読み込むCSVのヘッダー情報.
        /// </summary>
        private static string[] headers;

        /// <summary>
        /// CSVインポート時の関数.
        /// </summary>
        public static AnimationClip Import(string path, Transform target)
        {
            var dicMotionData = CreateMotionList(path);
            var dicBoneCurves = CreateCurves(target, dicMotionData);
            return CreateAnimationClip(target, dicBoneCurves);
        }

        /// <summary>
        /// CSVを読み取り内容をデータ化.
        /// </summary>
        private static Dictionary<float, Vector3[]> CreateMotionList(string csv)
        {

            Dictionary<float, Vector3[]> dicMotionData;

            using (StringReader sr = new StringReader(csv))
            {
                // ヘッダー情報読み取り.
                var headerLine = sr.ReadLine();
                headers = headerLine.Split(',');

                // 回転＋座標情報読み取り.
                dicMotionData = new Dictionary<float, Vector3[]>();
                var line = sr.ReadLine();
                while (sr.Peek() > -1)
                {
                    if (line == "")
                    {
                        line = sr.ReadLine();
                        continue;
                    }
                    var elements = line.Split(',');
                    var motiondatas = new Vector3[headerLine.Length - 1];
                    for (int i = 0; i < headers.Length - 1; i++)
                    {
                        if (i == 0)
                        {
                            continue;
                        }

                        var vector3Elements = elements[i].Split(' ');

                        var vector3Data = new Vector3
                            (ParseToRotation(vector3Elements[0]),
                            ParseToRotation(vector3Elements[1]),
                            ParseToRotation(vector3Elements[2])
                            );
                        motiondatas[i - 1] = vector3Data;
                    }
                    dicMotionData.Add(float.Parse(elements[0]), motiondatas);
                    line = sr.ReadLine();
                }
            }
            return dicMotionData;
        }

        /// <summary>
        /// stringをfloatに変換.
        /// </summary>
        private static float ParseToRotation(string value)
        {
            return float.Parse(value);
        }

        /// <summary>
        /// アニメーションカーブの作成.
        /// </summary>
        private static Dictionary<HumanBodyBones, AnimationCurve[]> CreateCurves(Transform target, Dictionary<float, Vector3[]> dicMotionData)
        {

            AnimationCurve[] GetRotationAnimationCurves()
            {
                var animationCurves = new AnimationCurve[CURVE_NUM];
                for (var i = 0; i < animationCurves.Length; i++)
                {
                    animationCurves[i] = new AnimationCurve();
                }
                return animationCurves;
            }

            AnimationCurve[] GetAnimationsCurves()
            {
                var animationCurves = new AnimationCurve[CURVE_NUM_HIPS];
                for (var i = 0; i < animationCurves.Length; i++)
                {
                    animationCurves[i] = new AnimationCurve();
                }
                return animationCurves;
            }

            Dictionary<HumanBodyBones, AnimationCurve[]> dicBoneCurve = new Dictionary<HumanBodyBones, AnimationCurve[]>
            {
                [HumanBodyBones.Hips] = GetAnimationsCurves(),
                [HumanBodyBones.Spine] = GetRotationAnimationCurves(),
                [HumanBodyBones.Chest] = GetRotationAnimationCurves(),
                [HumanBodyBones.Neck] = GetRotationAnimationCurves(),
                [HumanBodyBones.Head] = GetRotationAnimationCurves(),
                [HumanBodyBones.LeftEye] = GetRotationAnimationCurves(),
                [HumanBodyBones.RightEye] = GetRotationAnimationCurves(),
                [HumanBodyBones.LeftShoulder] = GetRotationAnimationCurves(),
                [HumanBodyBones.LeftUpperArm] = GetRotationAnimationCurves(),
                [HumanBodyBones.LeftLowerArm] = GetRotationAnimationCurves(),
                [HumanBodyBones.LeftHand] = GetRotationAnimationCurves(),
                [HumanBodyBones.RightShoulder] = GetRotationAnimationCurves(),
                [HumanBodyBones.RightUpperArm] = GetRotationAnimationCurves(),
                [HumanBodyBones.RightLowerArm] = GetRotationAnimationCurves(),
                [HumanBodyBones.RightHand] = GetRotationAnimationCurves(),
                [HumanBodyBones.LeftUpperLeg] = GetRotationAnimationCurves(),
                [HumanBodyBones.LeftLowerLeg] = GetRotationAnimationCurves(),
                [HumanBodyBones.LeftFoot] = GetRotationAnimationCurves(),
                [HumanBodyBones.LeftToes] = GetRotationAnimationCurves(),
                [HumanBodyBones.RightUpperLeg] = GetRotationAnimationCurves(),
                [HumanBodyBones.RightLowerLeg] = GetRotationAnimationCurves(),
                [HumanBodyBones.RightFoot] = GetRotationAnimationCurves(),
                [HumanBodyBones.RightToes] = GetRotationAnimationCurves()
            };

            //CSVデータをアバターパーツごとに分割格納.
            foreach (var keyValuePair in dicMotionData)
            {
                var hipPosition = new Vector3();

                for (int i = 0; i < headers.Length - 1; i++)
                {
                    // 1列目（時間情報）はスキップ.
                    if (i == 0)
                    {
                        continue;
                    }
                    // 2列目（Hipsの座標情報）の格納.
                    else if (i == 1)
                    {
                        hipPosition = keyValuePair.Value[i - 1];
                    }
                    // 3列目（Hipsの回転情報）の格納.
                    else if (i == 2)
                    {
                        HumanBodyBones humanBodyBones;
                        Enum.TryParse(headers[i], out humanBodyBones);
                        var animator = target.GetComponent<Animator>();
                        var bone = animator.GetBoneTransform(humanBodyBones);
                        if (bone == null)
                        {
                            continue;
                        }
                        var keyframes = GetKeyFrames(keyValuePair.Key, hipPosition, keyValuePair.Value[i - 1], bone);
                        for (var j = 0; j < dicBoneCurve[humanBodyBones].Length; j++)
                        {
                            dicBoneCurve[humanBodyBones][j].AddKey(keyframes[j]);
                        }
                    }
                    // 3列目（Hips以外の回転情報）の格納.
                    else
                    {
                        HumanBodyBones humanBodyBones;
                        Enum.TryParse(headers[i], out humanBodyBones);
                        var animator = target.GetComponent<Animator>();
                        var bone = animator.GetBoneTransform(humanBodyBones);
                        if (bone == null)
                        {
                            continue;
                        }
                        var keyframes = GetRotationKeyFrames(keyValuePair.Key, keyValuePair.Value[i - 1], bone);
                        for (var j = 0; j < dicBoneCurve[humanBodyBones].Length; j++)
                        {
                            dicBoneCurve[humanBodyBones][j].AddKey(keyframes[j]);
                        }
                    }
                }
            }
            return dicBoneCurve;
        }

        /// <summary>
        /// キーフレームの作成（回転情報のみ）.
        /// </summary>
        private static Keyframe[] GetRotationKeyFrames(float time, Vector3 eularAngle, Transform bone)
        {
            var initialRotation = bone.rotation;

            var addRotation = Quaternion.Euler(eularAngle);
            bone.rotation = addRotation * initialRotation;

            var localRotation = bone.localRotation;
            bone.rotation = initialRotation;

            var keyframes = new Keyframe[CURVE_NUM];
            keyframes[0] = new Keyframe(time, localRotation.x, 0.0f, 0.0f);
            keyframes[1] = new Keyframe(time, localRotation.y, 0.0f, 0.0f);
            keyframes[2] = new Keyframe(time, localRotation.z, 0.0f, 0.0f);
            keyframes[3] = new Keyframe(time, localRotation.w, 0.0f, 0.0f);
            return keyframes;
        }

        /// <summary>
        /// キーフレームの作成（回転＋座標情報）.
        /// </summary>
        private static Keyframe[] GetKeyFrames(float time, Vector3 position, Vector3 eularAngle, Transform bone)
        {
            var initialRotation = bone.rotation;

            var addRotation = Quaternion.Euler(eularAngle);
            bone.rotation = addRotation * initialRotation;

            var localRotation = bone.localRotation;
            bone.rotation = initialRotation;

            var keyframes = new Keyframe[7];
            keyframes[0] = new Keyframe(time, position.x, 0.0f, 0.0f);
            keyframes[1] = new Keyframe(time, position.y, 0.0f, 0.0f);
            keyframes[2] = new Keyframe(time, position.z, 0.0f, 0.0f);
            keyframes[3] = new Keyframe(time, localRotation.x, 0.0f, 0.0f);
            keyframes[4] = new Keyframe(time, localRotation.y, 0.0f, 0.0f);
            keyframes[5] = new Keyframe(time, localRotation.z, 0.0f, 0.0f);
            keyframes[6] = new Keyframe(time, localRotation.w, 0.0f, 0.0f);
            return keyframes;
        }

        /// <summary>
        /// アニメーションクリップの作成.
        /// </summary>
        private static AnimationClip CreateAnimationClip(Transform target, Dictionary<HumanBodyBones, AnimationCurve[]> dicBoneCurve)
        {
            var clip = new AnimationClip();
            clip.legacy = false;

            var animator = target.GetComponent<Animator>();

            // アニメーションカーブをセットするオブジェクトのファイルパス取得.
            string GetRootPath(Transform transform)
            {
                if (transform.parent == null)
                {
                    return $"";
                }
                else
                {
                    if (transform.GetComponent<Animator>() != null)
                    {
                        return $"";
                    }
                    var path = GetRootPath(transform.parent);
                    if (path == "")
                    {
                        return $"{transform.name}";
                    }
                    else
                    {
                        return $"{path}/{transform.name}";
                    }
                }
            }

            // クリップにパーツ毎のアニメーションカーブをセット.
            foreach (var keyValuePair in dicBoneCurve)
            {
                var bone = animator.GetBoneTransform(keyValuePair.Key);
                if (bone == null)
                {
                    continue;
                }

                var bonename = GetRootPath(bone);
                if (keyValuePair.Key == HumanBodyBones.Hips)
                {
                    clip.SetCurve(bonename, typeof(Transform), "localPosition.x", keyValuePair.Value[0]);
                    clip.SetCurve(bonename, typeof(Transform), "localPosition.y", keyValuePair.Value[1]);
                    clip.SetCurve(bonename, typeof(Transform), "localPosition.z", keyValuePair.Value[2]);

                    clip.SetCurve(bonename, typeof(Transform), "localRotation.x", keyValuePair.Value[3]);
                    clip.SetCurve(bonename, typeof(Transform), "localRotation.y", keyValuePair.Value[4]);
                    clip.SetCurve(bonename, typeof(Transform), "localRotation.z", keyValuePair.Value[5]);
                    clip.SetCurve(bonename, typeof(Transform), "localRotation.w", keyValuePair.Value[6]);
                }
                else
                {
                    clip.SetCurve(bonename, typeof(Transform), "localRotation.x", keyValuePair.Value[0]);
                    clip.SetCurve(bonename, typeof(Transform), "localRotation.y", keyValuePair.Value[1]);
                    clip.SetCurve(bonename, typeof(Transform), "localRotation.z", keyValuePair.Value[2]);
                    clip.SetCurve(bonename, typeof(Transform), "localRotation.w", keyValuePair.Value[3]);
                }

            }
            /// <summary>
            /// アニメーションクリップの補完．
            /// </summary>
            clip.EnsureQuaternionContinuity();

            return clip;
        }

    }
}