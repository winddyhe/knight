#if UNITY_EDITOR
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Knight.Framework.ShaderVariant
{
    [ExecuteInEditMode]
    public class LightEnv : MonoBehaviour
    {
        public Light m_mainLight;
        public Light m_vertexLight;
        public ReflectionProbe m_reflectionProbe;

        public bool m_builtinShadowSupport = false;

        object m_status = null;
        IEnumerator m_statusIT = null;

        /// <summary>
        /// 枚举当前场景所有的光照开关状态，如实时阴影，实时灯光，顶点光照，雾，反射探针，光照探针
        /// </summary>
        /// <returns></returns>
        IEnumerator GetSwitchStatus()
        {
            var switchers = new List<KeyValuePair<Action, Action>>();
            if (m_mainLight != null)
            {
                switchers.Add(
                    new KeyValuePair<Action, Action>(
                        () =>
                        {
                            m_mainLight.gameObject.SetActive(true);
                        },
                        () =>
                        {
                            m_mainLight.gameObject.SetActive(false);
                        }
                    )
                );
                if (m_builtinShadowSupport)
                {
                    switchers.Add(
                        new KeyValuePair<Action, Action>(
                            () =>
                            {
                                m_mainLight.gameObject.SetActive(true);
                                m_mainLight.shadows = LightShadows.Soft;
                                m_mainLight.shadowResolution = UnityEngine.Rendering.LightShadowResolution.Low;
                            },
                            () =>
                            {
                                m_mainLight.gameObject.SetActive(false);
                                m_mainLight.shadows = LightShadows.None;
                                m_mainLight.shadowResolution = UnityEngine.Rendering.LightShadowResolution.FromQualitySettings;
                            }
                        )
                    );
                }
            }
            if (m_vertexLight != null)
            {
                switchers.Add(
                    new KeyValuePair<Action, Action>(
                        () =>
                        {
                            m_vertexLight.gameObject.SetActive(true);
                            var vertexLights = UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None).Where(
                                l => l.renderMode == LightRenderMode.ForceVertex || l.type != LightType.Directional).ToList();
                            vertexLights.ForEach(l => l.gameObject.SetActive(true));
                        },
                        () =>
                        {
                            m_vertexLight.gameObject.SetActive(false);
                            var vertexLights = UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None).Where(
                                l => l.renderMode == LightRenderMode.ForceVertex || l.type == LightType.Directional).ToList();
                        }
                    )
                );
            }
            if (m_reflectionProbe != null)
            {
                switchers.Add(
                    new KeyValuePair<Action, Action>(
                        () =>
                        {
                            m_reflectionProbe.gameObject.SetActive(true);
                            var reflectionProbs = UnityEngine.Object.FindObjectsByType<ReflectionProbe>(FindObjectsSortMode.None).ToList();
                            reflectionProbs.ForEach(l => l.gameObject.SetActive(true));
                        },
                        () =>
                        {
                            m_reflectionProbe.gameObject.SetActive(false);
                            var reflectionProbs = UnityEngine.Object.FindObjectsByType<ReflectionProbe>(FindObjectsSortMode.None).ToList();
                            reflectionProbs.ForEach(l => l.gameObject.SetActive(false));
                        }
                    )
                );
            }
            var count = 1 << switchers.Count;
            for (int i = 0; i < count; ++i)
            {
                for (int j = 0; j < switchers.Count; ++j)
                {
                    if ((i & (1 << j)) != 0)
                    {
                        switchers[j].Key();
                    }
                    else
                    {
                        if (switchers[j].Value != null)
                        {
                            switchers[j].Value();
                        }
                        else
                        {
                            switchers[j].Key();
                        }
                    }
                }
                yield return i;
            }
        }

        Color GetButtonColor()
        {
            if (m_statusIT == null)
            {
                return Color.white;
            }
            else
            {
                return Color.red;
            }
        }

        public String GetStatusText()
        {
            return String.Format("Status: {0}", m_status != null ? m_status.ToString() : "null");
        }

#if ODIN_INSPECTOR
        [GUIColor( "GetButtonColor" )]
        [Button( "$GetStatusText", ButtonSizes.Gigantic )]
#endif
        public bool SwitchStatus()
        {
            if (m_statusIT == null)
            {
                m_status = null;
                m_statusIT = GetSwitchStatus();
            }
            if (!m_statusIT.MoveNext())
            {
                m_statusIT = null;
                m_status = null;
                return false;
            }
            else
            {
                m_status = m_statusIT.Current;
            }
            return true;
        }

        void OnEnable()
        {
            m_mainLight = null;
            m_vertexLight = null;
            var lights = UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
            for (int i = 0; i < lights.Length; ++i)
            {
                var light = lights[i];
                if (light.isActiveAndEnabled)
                {
                    if (light.renderMode == LightRenderMode.ForcePixel && light.type == LightType.Directional)
                    {
                        // 只记录第一个重要的平行光，其他的关闭
                        if (m_mainLight == null)
                        {
                            m_mainLight = light;
                        }
                        else if (m_mainLight != light)
                        {
                            light.enabled = false;
                        }
                    }
                    else if (light.renderMode == LightRenderMode.ForceVertex)
                    {
                        m_vertexLight = light;
                    }
                    else
                    {
                        light.enabled = false;
                    }
                }
            }
            m_reflectionProbe = UnityEngine.Object.FindObjectsByType<ReflectionProbe>(FindObjectsSortMode.None).FirstOrDefault();
        }
    }
}
#endif
//EOF
