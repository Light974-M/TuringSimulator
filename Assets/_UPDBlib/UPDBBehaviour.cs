using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods.Structures;

namespace UPDB.CoreHelper.UsableMethods
{
    ///<summary>
    /// UPDB methods that does not use extensions, callable in every classes that derives from monoBehaviour
    ///</summary>
    public class UPDBBehaviour : MonoBehaviour
    {
        /******************************************************NATIVE METHODS***********************************************************/

        /// <summary>
        /// call when scene is updating before rendering gizmos
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                OnScene();
        }

        /// <summary>
        /// call when scene is updating before rendering gizmos if obhect is selected by inspector
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
                OnSceneSelected();
        }


        /******************************************************CUSTOM METHODS***********************************************************/

        /// <summary>
        /// called on scene update in editor only
        /// </summary>
        protected virtual void OnScene()
        {

        }

        /// <summary>
        /// called on scene update in editor only, when object is selected by inspector
        /// </summary>
        protected virtual void OnSceneSelected()
        {

        }

        /******************************************************UTILITY METHODS**********************************************************/

        /// <summary>
        /// try to find Object, and, if not, let an exception parameter
        /// </summary>
        /// <param name="variable">variable that will assign the TryFindObjectOfType value</param>
        /// <returns></returns>
        public static bool TryFindObjectOfType<T>(out T variable) where T : Object
        {
            variable = FindObjectOfType<T>();
            return variable != null;
        }

        /// <summary>
        /// return if two lists have exactly the sames values(same number of objects and same objects at same positions)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        public static bool AreSameList<T>(List<T> list1, List<T> list2) where T : UnityEngine.Object
        {
            bool isSame = true;

            if (list1.Count == list2.Count)
            {
                for (int i = 0; i < list1.Count; i++)
                    if (list1[i] != list2[i])
                        isSame = false;
            }
            else
            {
                isSame = false;
            }

            return isSame;
        }

        /// <summary>
        /// calculate an angularVelocity from transform rotation of a moving object
        /// </summary>
        /// <param name="rotation">actual rotation</param>
        /// <param name="previousRotation">last rotation(only need a variable, value is automatically updated</param>
        /// <param name="timeDelta">is deltaTime, fixedDeltaTime, or other ?</param>
        /// <returns></returns>
        public static Vector3 GetAngularVelocity(Quaternion rotation, ref Quaternion previousRotation, float timeDelta)
        {
            Quaternion deltaRotation = rotation * Quaternion.Inverse(previousRotation);

            deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);

            angle *= Mathf.Deg2Rad;

            Vector3 angularVelocity = (1.0f / timeDelta) * angle * axis;

            previousRotation = rotation;

            return angularVelocity;
        }

        /// <summary>
        /// destroy instance in play or in editor intelligently
        /// </summary>
        /// <param name="obj"></param>
        public static void IntelliDestroy(Object obj)
        {
            if (Application.isPlaying)
                Destroy(obj);
            else
                DestroyImmediate(obj);
        }

        /// <summary>
        /// make the value given "clamp" but instead of clamping to borders, it is looping, so it's clamping with "inverse borns" usefull when using arrays and want to loop without going outside the bounds
        /// </summary>
        /// <param name="value">value to clamp</param>
        /// <param name="min">min born(0 for a list)</param>
        /// <param name="max">max born(length - 1 for a list)</param>
        /// <returns></returns>
        public static int LoopClamp(int value, int min, int max)
        {
            bool isLastValueOutbound = value > max;
            bool isFirstValueOutboud = value < min;

            if (isLastValueOutbound)
            {
                return min;
            }

            if (isFirstValueOutboud)
            {
                return max;
            }

            if (!isFirstValueOutboud && !isLastValueOutbound)
            {
                return value;
            }

            return value;
        }

        /// <summary>
        /// make a (not really optimized) find object of type, but for generic classes, it will search if any object in scene are derived from the class<T> given
        /// </summary>
        /// <typeparam name="T">parent class, if none, put System.Object</typeparam>
        /// <param name="type">generic class to search for</param>
        /// <returns></returns>
        public static T[] FindObjectsOfTypeGeneric<T>(System.Type type) where T : Object
        {
            T[] objList = FindObjectsOfType<T>();
            List<T> matchList = new List<T>();

            foreach (T obj in objList)
            {
                System.Type objTypeRaw = obj.GetType();

                while (objTypeRaw != typeof(System.Object))
                {
                    objTypeRaw = objTypeRaw.BaseType;
                    System.Type objType = objTypeRaw.IsGenericType ? objTypeRaw.GetGenericTypeDefinition() : objTypeRaw;

                    if (objType == type)
                    {
                        matchList.Add(obj);
                        break;
                    }
                }
            }

            return matchList.ToArray();
        }

        #region AutoLerp

        /// <summary>
        /// make a lerp automatically(based on delta time update rate) with float, between a and b, in a specific time
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// <returns></returns>
        public static float AutoLerp(float a, float b, float lerpTime, ref float timer)
        {
            float value = 0;

            if (timer < lerpTime)
            {
                value = Mathf.Lerp(a, b, timer / lerpTime);
                timer += Time.deltaTime;
            }
            else
            {
                timer = lerpTime;
                value = b;
            }

            return value;
        }

        /// <summary>
        /// make a lerp automatically(based on delta time update rate) with Vector2, between a and b, in a specific time
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// <returns></returns>
        public static Vector2 AutoLerp(Vector2 a, Vector2 b, float lerpTime, ref float timer)
        {
            Vector2 value = Vector2.zero;

            if (timer < lerpTime)
            {
                value = Vector2.Lerp(a, b, timer / lerpTime);
                timer += Time.deltaTime;
            }
            else
            {
                timer = lerpTime;
                value = b;
            }

            return value;
        }

        /// <summary>
        /// make a lerp automatically(based on delta time update rate) with Vector3, between a and b, in a specific time
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// <returns></returns>
        public static Vector3 AutoLerp(Vector3 a, Vector3 b, float lerpTime, ref float timer)
        {
            Vector3 value = Vector3.zero;

            if (timer < lerpTime)
            {
                value = Vector3.Lerp(a, b, timer / lerpTime);
                timer += Time.deltaTime;
            }
            else
            {
                timer = lerpTime;
                value = b;
            }

            return value;
        }

        /// <summary>
        /// make a lerp automatically(based on delta time update rate) with Vector4, between a and b, in a specific time
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// <returns></returns>
        public static Vector4 AutoLerp(Vector4 a, Vector4 b, float lerpTime, ref float timer)
        {
            Vector4 value = Vector4.zero;

            if (timer < lerpTime)
            {
                value = Vector4.Lerp(a, b, timer / lerpTime);
                timer += Time.deltaTime;
            }
            else
            {
                timer = lerpTime;
                value = b;
            }

            return value;
        }

        /// <summary>
        /// make a rotation lerp automatically(based on delta time update rate) with Quaternions, between a and b, in a specific time
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// <returns></returns>
        public static Quaternion AutoLerp(Quaternion a, Quaternion b, float lerpTime, ref float timer)
        {
            Quaternion value = Quaternion.identity;

            if (timer < lerpTime)
            {
                value = Quaternion.Lerp(a, b, timer / lerpTime);
                timer += Time.deltaTime;
            }
            else
            {
                timer = lerpTime;
                value = b;
            }

            return value;
        }

        /// <summary>
        /// make a lerp automatically(based on delta time update rate) with float, between a and b, in a specific time
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// /// <param name="smoothTimer">curve to offset timer and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns></returns>
        public static float AutoLerp(float a, float b, float lerpTime, ref float timer, AnimationCurve smoothTimer)
        {
            float value = 0;

            if (timer < lerpTime)
            {
                //normalize value of timer for curve time
                float lastKeyTime = smoothTimer.keys[smoothTimer.length - 1].time;
                float firstKeyTime = smoothTimer.keys[0].time;
                float timerBaseValue = (timer / lerpTime);

                float timerScaledValue = (timerBaseValue * (lastKeyTime - firstKeyTime)) + firstKeyTime;
                float timerCurveValue = smoothTimer.Evaluate(timerScaledValue);

                //normalize also value of curve
                float lastKeyValue = smoothTimer.keys[smoothTimer.length - 1].value;
                float firstKeyValue = smoothTimer.keys[0].value;

                float timerCurveValueNormalized = (timerCurveValue - firstKeyValue) / (lastKeyValue - firstKeyValue);

                value = Mathf.Lerp(a, b, timerCurveValueNormalized);
                timer += Time.deltaTime;
            }
            else
            {
                timer = lerpTime;
                value = b;
            }

            return value;
        }

        /// <summary>
        /// make a lerp automatically(based on delta time update rate) with Vector2, between a and b, in a specific time, with an addition of choosing the shape of a non linear lerp
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// /// <param name="smoothTimer">curve to offset timer and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns></returns>
        public static Vector2 AutoLerp(Vector2 a, Vector2 b, float lerpTime, ref float timer, AnimationCurve smoothTimer)
        {
            //create a null vector 2
            Vector2 value = Vector2.zero;

            //if timer has not reach lerp time, update lerp state, if it has, put timer to lerp time, and value to end state
            if (timer < lerpTime)
            {
                //normalize value of timer for curve time
                float lastKeyTime = smoothTimer.keys[smoothTimer.length - 1].time;
                float firstKeyTime = smoothTimer.keys[0].time;
                float timerBaseValue = (timer / lerpTime);

                float timerScaledValue = (timerBaseValue * (lastKeyTime - firstKeyTime)) + firstKeyTime;
                float timerCurveValue = smoothTimer.Evaluate(timerScaledValue);

                //normalize also value of curve
                float lastKeyValue = smoothTimer.keys[smoothTimer.length - 1].value;
                float firstKeyValue = smoothTimer.keys[0].value;

                float timerCurveValueNormalized = (timerCurveValue - firstKeyValue) / (lastKeyValue - firstKeyValue);

                //update value to a lerp between a and b, with timer / lerp timer for T (value between 0 and 1) applyied to the animation curve value
                value = Vector2.Lerp(a, b, timerCurveValueNormalized);

                //update timer value
                timer += Time.deltaTime;
            }
            else
            {
                timer = lerpTime;
                value = b;
            }

            //return value updated
            return value;
        }

        /// <summary>
        /// make a lerp automatically(based on delta time update rate) with Vector3, between a and b, in a specific time
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// /// <param name="smoothTimer">curve to offset timer and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns></returns>
        public static Vector3 AutoLerp(Vector3 a, Vector3 b, float lerpTime, ref float timer, AnimationCurve smoothTimer)
        {
            Vector3 value = Vector3.zero;

            if (timer < lerpTime)
            {
                //normalize value of timer for curve time
                float lastKeyTime = smoothTimer.keys[smoothTimer.length - 1].time;
                float firstKeyTime = smoothTimer.keys[0].time;
                float timerBaseValue = (timer / lerpTime);

                float timerScaledValue = (timerBaseValue * (lastKeyTime - firstKeyTime)) + firstKeyTime;
                float timerCurveValue = smoothTimer.Evaluate(timerScaledValue);

                //normalize also value of curve
                float lastKeyValue = smoothTimer.keys[smoothTimer.length - 1].value;
                float firstKeyValue = smoothTimer.keys[0].value;

                float timerCurveValueNormalized = (timerCurveValue - firstKeyValue) / (lastKeyValue - firstKeyValue);

                value = Vector3.Lerp(a, b, timerCurveValueNormalized);
                timer += Time.deltaTime;
            }
            else
            {
                timer = lerpTime;
                value = b;
            }

            return value;
        }

        /// <summary>
        /// make a lerp automatically(based on delta time update rate) with Vector4, between a and b, in a specific time
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// /// <param name="smoothTimer">curve to offset timer and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns></returns>
        public static Vector4 AutoLerp(Vector4 a, Vector4 b, float lerpTime, ref float timer, AnimationCurve smoothTimer)
        {
            Vector4 value = Vector4.zero;

            if (timer < lerpTime)
            {
                //normalize value of timer for curve time
                float lastKeyTime = smoothTimer.keys[smoothTimer.length - 1].time;
                float firstKeyTime = smoothTimer.keys[0].time;
                float timerBaseValue = (timer / lerpTime);

                float timerScaledValue = (timerBaseValue * (lastKeyTime - firstKeyTime)) + firstKeyTime;
                float timerCurveValue = smoothTimer.Evaluate(timerScaledValue);

                //normalize also value of curve
                float lastKeyValue = smoothTimer.keys[smoothTimer.length - 1].value;
                float firstKeyValue = smoothTimer.keys[0].value;

                float timerCurveValueNormalized = (timerCurveValue - firstKeyValue) / (lastKeyValue - firstKeyValue);

                value = Vector4.Lerp(a, b, timerCurveValueNormalized);

                timer += Time.deltaTime;
            }
            else
            {
                timer = lerpTime;
                value = b;
            }

            return value;
        }

        /// <summary>
        /// make a rotation lerp automatically(based on delta time update rate) with Quaternions, between a and b, in a specific time
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="lerpTime">time to make lerp</param>
        /// <param name="timer">timer to store lerp progression</param>
        /// /// <param name="smoothTimer">curve to offset timer and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns></returns>
        public static Quaternion AutoLerp(Quaternion a, Quaternion b, float lerpTime, ref float timer, AnimationCurve smoothTimer)
        {
            Quaternion value = Quaternion.identity;

            if (timer < lerpTime)
            {
                //normalize value of timer for curve time
                float lastKeyTime = smoothTimer.keys[smoothTimer.length - 1].time;
                float firstKeyTime = smoothTimer.keys[0].time;
                float timerBaseValue = (timer / lerpTime);

                float timerScaledValue = (timerBaseValue * (lastKeyTime - firstKeyTime)) + firstKeyTime;
                float timerCurveValue = smoothTimer.Evaluate(timerScaledValue);

                //normalize also value of curve
                float lastKeyValue = smoothTimer.keys[smoothTimer.length - 1].value;
                float firstKeyValue = smoothTimer.keys[0].value;

                float timerCurveValueNormalized = (timerCurveValue - firstKeyValue) / (lastKeyValue - firstKeyValue);

                value = Quaternion.Lerp(a, b, timerCurveValueNormalized);

                timer += Time.deltaTime;
            }
            else
            {
                timer = lerpTime;
                value = b;
            }

            return value;
        }

        #endregion

        #region CurveLerp

        /// <summary>
        /// make a lerp with an added curve shape to time value
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="t">time t representing lerp progression</param>
        /// /// <param name="shape">curve to offset t and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns></returns>
        public static float CurveLerp(float a, float b, float t, AnimationCurve shape)
        {
            //normalize value of timer for curve time
            float lastKeyTime = shape.keys[shape.length - 1].time;
            float firstKeyTime = shape.keys[0].time;

            float timeScaledValue = (t * (lastKeyTime - firstKeyTime)) + firstKeyTime;
            float timeCurveValue = shape.Evaluate(timeScaledValue);

            //normalize also value of curve
            float lastKeyValue = shape.keys[shape.length - 1].value;
            float firstKeyValue = shape.keys[0].value;

            float timerCurveValueNormalized = (timeCurveValue - firstKeyValue) / (lastKeyValue - firstKeyValue);

            return Mathf.Lerp(a, b, timerCurveValueNormalized);
        }

        /// <summary>
        /// make a lerp with an added curve shape to time value
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="t">time t representing lerp progression</param>
        /// /// <param name="shape">curve to offset t and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns></returns>
        public static Vector2 CurveLerp(Vector2 a, Vector2 b, float t, AnimationCurve shape)
        {
            //normalize value of timer for curve time
            float lastKeyTime = shape.keys[shape.length - 1].time;
            float firstKeyTime = shape.keys[0].time;

            float timeScaledValue = (t * (lastKeyTime - firstKeyTime)) + firstKeyTime;
            float timeCurveValue = shape.Evaluate(timeScaledValue);

            //normalize also value of curve
            float lastKeyValue = shape.keys[shape.length - 1].value;
            float firstKeyValue = shape.keys[0].value;

            float timerCurveValueNormalized = (timeCurveValue - firstKeyValue) / (lastKeyValue - firstKeyValue);

            return Vector2.Lerp(a, b, timerCurveValueNormalized);
        }

        /// <summary>
        /// make a lerp with an added curve shape to time value
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="t">time t representing lerp progression</param>
        /// /// <param name="shape">curve to offset t and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns></returns>
        public static Vector3 CurveLerp(Vector3 a, Vector3 b, float t, AnimationCurve shape)
        {
            //normalize value of timer for curve time
            float lastKeyTime = shape.keys[shape.length - 1].time;
            float firstKeyTime = shape.keys[0].time;

            float timeScaledValue = (t * (lastKeyTime - firstKeyTime)) + firstKeyTime;
            float timeCurveValue = shape.Evaluate(timeScaledValue);

            //normalize also value of curve
            float lastKeyValue = shape.keys[shape.length - 1].value;
            float firstKeyValue = shape.keys[0].value;

            float timerCurveValueNormalized = (timeCurveValue - firstKeyValue) / (lastKeyValue - firstKeyValue);

            return Vector3.Lerp(a, b, timerCurveValueNormalized);
        }

        /// <summary>
        /// make a lerp with an added curve shape to time value
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="t">time t representing lerp progression</param>
        /// /// <param name="shape">curve to offset t and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns></returns>
        public static Vector4 CurveLerp(Vector4 a, Vector4 b, float t, AnimationCurve shape)
        {
            //normalize value of timer for curve time
            float lastKeyTime = shape.keys[shape.length - 1].time;
            float firstKeyTime = shape.keys[0].time;

            float timeScaledValue = (t * (lastKeyTime - firstKeyTime)) + firstKeyTime;
            float timeCurveValue = shape.Evaluate(timeScaledValue);

            //normalize also value of curve
            float lastKeyValue = shape.keys[shape.length - 1].value;
            float firstKeyValue = shape.keys[0].value;

            float timerCurveValueNormalized = (timeCurveValue - firstKeyValue) / (lastKeyValue - firstKeyValue);

            return Vector4.Lerp(a, b, timerCurveValueNormalized);
        }

        /// <summary>
        /// make a lerp with an added curve shape to time value
        /// </summary>
        /// <param name="a">start point</param>
        /// <param name="b">end point</param>
        /// <param name="t">time t representing lerp progression</param>
        /// /// <param name="shape">curve to offset t and make things smooth(min and max time and value doesn't do anything, just focus on curve shape)</param>
        /// <returns></returns>
        public static Quaternion CurveLerp(Quaternion a, Quaternion b, float t, AnimationCurve shape)
        {
            //normalize value of timer for curve time
            float lastKeyTime = shape.keys[shape.length - 1].time;
            float firstKeyTime = shape.keys[0].time;

            float timeScaledValue = (t * (lastKeyTime - firstKeyTime)) + firstKeyTime;
            float timeCurveValue = shape.Evaluate(timeScaledValue);

            //normalize also value of curve
            float lastKeyValue = shape.keys[shape.length - 1].value;
            float firstKeyValue = shape.keys[0].value;

            float timerCurveValueNormalized = (timeCurveValue - firstKeyValue) / (lastKeyValue - firstKeyValue);

            return Quaternion.Lerp(a, b, timerCurveValueNormalized);
        }

        #endregion

        #region Custom LookRotation

        /// <summary>
        /// calculate rotation with a direction vector and a up vector
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static Quaternion MyLookRotation(Vector3 dir, Vector3 up)
        {
            if (dir == Vector3.zero)
            {
                Debug.Log("Zero direction in MyLookRotation");
                return Quaternion.identity;
            }

            if (up != dir)
            {
                up.Normalize();
                var v = dir + up * -Vector3.Dot(up, dir);
                var q = Quaternion.FromToRotation(Vector3.forward, v);
                return Quaternion.FromToRotation(v, dir) * q;
            }
            else
            {
                return Quaternion.FromToRotation(Vector3.forward, dir);
            }
        }

        /// <summary>
        /// make a lookAt rotation with up instead(turn a pleyr to ave feet pointing direction)
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="dir"></param>
        public static void LookRotationUp(Transform transform, Vector3 dir)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.right, dir);
            transform.Rotate(0, -90, 0);
            transform.rotation = Quaternion.LookRotation(transform.forward, dir);
        }

        #endregion


        #region Vector2CoordinateConversion

        #region Vector type conversions

        #region WorldToLocal overrides

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in world axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <returns></returns>
        public static Vector2 VecWorldToLocal(Vector2 vecToConvert, Vector2 xAxis, Vector2 yAxis)
        {
            return ConvertVectorFromSystemAToSystemB(vecToConvert, xAxis, yAxis);
        }

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in world axis</param>
        /// <param name="axis">axis representing local x or y axis</param>
        /// <param name="axisToFind">is the vector above representing x or y ?</param>
        /// <returns></returns>
        public static Vector2 VecWorldToLocal(Vector2 vecToConvert, Vector2 axis, Axis axisToFind)
        {
            return ConvertVectorFromSystemAToSystemB(vecToConvert, axis, axisToFind);
        }

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in world axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector2 VecWorldToLocal(Vector2 vecToConvert, Vector2 xAxis, Vector2 yAxis, bool ignoreScale)
        {
            return ConvertVectorFromSystemAToSystemB(vecToConvert, xAxis, yAxis, ignoreScale);
        }

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in world axis</param>
        /// <param name="axis">axis representing local x or y axis</param>
        /// <param name="axisToFind">is the vector above representing x or y ?</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector2 VecWorldToLocal(Vector2 vecToConvert, Vector2 axis, Axis axisToFind, bool ignoreScale)
        {
            return ConvertVectorFromSystemAToSystemB(vecToConvert, axis, axisToFind, ignoreScale);
        }

        #endregion

        #region localToWorld overrides

        /// <summary>
        /// override for ConvertVectorToSystemBFromSystemA, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in local axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <returns></returns>
        public static Vector2 VecLocalToWorld(Vector2 vecToConvert, Vector2 xAxis, Vector2 yAxis)
        {
            return ConvertVectorToSystemBFromSystemA(vecToConvert, xAxis, yAxis);
        }

        /// <summary>
        /// override for ConvertVectorToSystemBFromSystemA, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in local axis</param>
        /// <param name="axis">axis representing local x or y axis</param>
        /// <param name="axisToFind">is the vector above representing x or y ?</param>
        /// <returns></returns>
        public static Vector2 VecLocalToWorld(Vector2 vecToConvert, Vector2 axis, Axis axisToFind)
        {
            return ConvertVectorToSystemBFromSystemA(vecToConvert, axis, axisToFind);
        }

        /// <summary>
        /// override for ConvertVectorToSystemBFromSystemA, basically just to remember wich method to use
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in local axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector2 VecLocalToWorld(Vector2 vecToConvert, Vector2 xAxis, Vector2 yAxis, bool ignoreScale)
        {
            return ConvertVectorToSystemBFromSystemA(vecToConvert, xAxis, yAxis, ignoreScale);
        }

        /// <summary>
        /// override for ConvertVectorToSystemBFromSystemA, basically just to remember wich method to use
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in local axis</param>
        /// <param name="axis">axis representing local x or y axis</param>
        /// <param name="axisToFind">is the vector above representing x or y ?</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector2 VecLocalToWorld(Vector2 vecToConvert, Vector2 axis, Axis axisToFind, bool ignoreScale)
        {
            return ConvertVectorToSystemBFromSystemA(vecToConvert, axis, axisToFind, ignoreScale);
        }

        #endregion

        #region from A to B 

        /// <summary>
        /// convert a given vector from coordinate system A to B
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="xAxisOfB">axis representing x axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing y axis of system B, given in system A</param>
        /// <returns></returns>
        public static Vector2 ConvertVectorFromSystemAToSystemB(Vector2 vecToConvert, Vector2 xAxisOfB, Vector2 yAxisOfB)
        {
            Vector2 xAxisNormalized = xAxisOfB.normalized;
            Vector2 yAxisNormalized = yAxisOfB.normalized;
            Vector2 axisMag = new Vector2(xAxisOfB.magnitude, yAxisOfB.magnitude);

            float x = (vecToConvert.x * xAxisNormalized.x) + (vecToConvert.y * xAxisNormalized.y);
            float y = (vecToConvert.x * yAxisNormalized.x) + (vecToConvert.y * yAxisNormalized.y);
            Vector2 convertedNormalizedVec = new Vector2(x, y);

            Vector2 convertedVec = convertedNormalizedVec / axisMag;

            return convertedVec;
        }

        /// <summary>
        /// convert a given vector from coordinate system A to B
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="axisOfB">axis representing x or y axis of system B, given in system A</param>
        /// <param name="axisToFind">is axis given above representing x or y ?</param>
        /// <returns></returns>
        public static Vector2 ConvertVectorFromSystemAToSystemB(Vector2 vecToConvert, Vector2 axisOfB, Axis axisToFind)
        {
            Vector2 xAxisOfB = Vector2.zero;
            Vector2 yAxisOfB = Vector2.zero;

            FindPerpToVec(axisOfB, axisToFind, ref xAxisOfB, ref yAxisOfB);

            Vector2 xAxisNormalized = xAxisOfB.normalized;
            Vector2 yAxisNormalized = yAxisOfB.normalized;
            Vector2 axisMag = new Vector2(xAxisOfB.magnitude, yAxisOfB.magnitude);

            float x = (vecToConvert.x * xAxisNormalized.x) + (vecToConvert.y * xAxisNormalized.y);
            float y = (vecToConvert.x * yAxisNormalized.x) + (vecToConvert.y * yAxisNormalized.y);
            Vector2 convertedNormalizedVec = new Vector2(x, y);

            Vector2 convertedVec = convertedNormalizedVec / axisMag;

            return convertedVec;
        }

        /// <summary>
        /// convert a given vector from coordinate system A to B
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="xAxisOfB">axis representing x axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing y axis of system B, given in system A</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector2 ConvertVectorFromSystemAToSystemB(Vector2 vecToConvert, Vector2 xAxisOfB, Vector2 yAxisOfB, bool ignoreScale)
        {
            Vector2 xAxisNormalized = xAxisOfB.normalized;
            Vector2 yAxisNormalized = yAxisOfB.normalized;
            Vector2 axisMag = new Vector2(xAxisOfB.magnitude, yAxisOfB.magnitude);

            float x = (vecToConvert.x * xAxisNormalized.x) + (vecToConvert.y * xAxisNormalized.y);
            float y = (vecToConvert.x * yAxisNormalized.x) + (vecToConvert.y * yAxisNormalized.y);
            Vector2 convertedNormalizedVec = new Vector2(x, y);

            Vector2 convertedVec = ignoreScale ? convertedNormalizedVec : convertedNormalizedVec / axisMag;

            return convertedVec;
        }

        /// <summary>
        /// convert a given vector from coordinate system A to B
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="axisOfB">axis representing x or y axis of system B, given in system A</param>
        /// <param name="axisToFind">is axis given above representing x or y ?</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector2 ConvertVectorFromSystemAToSystemB(Vector2 vecToConvert, Vector2 axisOfB, Axis axisToFind, bool ignoreScale)
        {
            Vector2 xAxisOfB = Vector2.zero;
            Vector2 yAxisOfB = Vector2.zero;

            FindPerpToVec(axisOfB, axisToFind, ref xAxisOfB, ref yAxisOfB);

            Vector2 xAxisNormalized = xAxisOfB.normalized;
            Vector2 yAxisNormalized = yAxisOfB.normalized;
            Vector2 axisMag = new Vector2(xAxisOfB.magnitude, yAxisOfB.magnitude);

            float x = (vecToConvert.x * xAxisNormalized.x) + (vecToConvert.y * xAxisNormalized.y);
            float y = (vecToConvert.x * yAxisNormalized.x) + (vecToConvert.y * yAxisNormalized.y);
            Vector2 convertedNormalizedVec = new Vector2(x, y);

            Vector2 convertedVec = ignoreScale ? convertedNormalizedVec : convertedNormalizedVec / axisMag;

            return convertedVec;
        }

        #endregion

        #region to B from A

        /// <summary>
        /// convert a given vector to coordinate system B from A
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="xAxisOfA">axis representing x axis of system A, given in system B</param>
        /// <param name="yAxisOfA">axis representing y axis of system A, given in system B</param>
        /// <returns></returns>
        public static Vector2 ConvertVectorToSystemBFromSystemA(Vector2 vecToConvert, Vector2 xAxisOfA, Vector2 yAxisOfA)
        {
            Vector2 xAxisNormalized = xAxisOfA.normalized;
            Vector2 yAxisNormalized = yAxisOfA.normalized;
            Vector2 axisMag = new Vector2(xAxisOfA.magnitude, yAxisOfA.magnitude);

            Vector2 VecNormalized = vecToConvert * axisMag;

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y);
            Vector2 convertedVec = new Vector2(x, y);


            return convertedVec;
        }

        /// <summary>
        /// convert a given vector to coordinate system B from A
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="axisOfA">axis representing x or y axis of system A, given in system B</param>
        /// <param name="axisToFind">is axis given above representing x or y ?</param>
        /// <returns></returns>
        public static Vector2 ConvertVectorToSystemBFromSystemA(Vector2 vecToConvert, Vector2 axisOfA, Axis axisToFind)
        {
            Vector2 xAxisOfA = Vector2.zero;
            Vector2 yAxisOfA = Vector2.zero;

            FindPerpToVec(axisOfA, axisToFind, ref xAxisOfA, ref yAxisOfA);

            Vector2 xAxisNormalized = xAxisOfA.normalized;
            Vector2 yAxisNormalized = yAxisOfA.normalized;
            Vector2 axisMag = new Vector2(xAxisOfA.magnitude, yAxisOfA.magnitude);

            Vector2 VecNormalized = vecToConvert * axisMag;

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y);
            Vector2 convertedVec = new Vector2(x, y);


            return convertedVec;
        }

        /// <summary>
        /// convert a given vector to coordinate system B from A
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="xAxisOfA">axis representing x axis of system A, given in system B</param>
        /// <param name="yAxisOfA">axis representing y axis of system A, given in system B</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector2 ConvertVectorToSystemBFromSystemA(Vector2 vecToConvert, Vector2 xAxisOfA, Vector2 yAxisOfA, bool ignoreScale)
        {
            Vector2 xAxisNormalized = xAxisOfA.normalized;
            Vector2 yAxisNormalized = yAxisOfA.normalized;
            Vector2 axisMag = new Vector2(xAxisOfA.magnitude, yAxisOfA.magnitude);

            Vector2 VecNormalized = ignoreScale ? vecToConvert : vecToConvert * axisMag;

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y);
            Vector2 convertedVec = new Vector2(x, y);


            return convertedVec;
        }

        /// <summary>
        /// convert a given vector to coordinate system B from A
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="axisOfA">axis representing x or y axis of system A, given in system B</param>
        /// <param name="axisToFind">is axis given above representing x or y ?</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector2 ConvertVectorToSystemBFromSystemA(Vector2 vecToConvert, Vector2 axisOfA, Axis axisToFind, bool ignoreScale)
        {
            Vector2 xAxisOfA = Vector2.zero;
            Vector2 yAxisOfA = Vector2.zero;

            FindPerpToVec(axisOfA, axisToFind, ref xAxisOfA, ref yAxisOfA);

            Vector2 xAxisNormalized = xAxisOfA.normalized;
            Vector2 yAxisNormalized = yAxisOfA.normalized;
            Vector2 axisMag = new Vector2(xAxisOfA.magnitude, yAxisOfA.magnitude);

            Vector2 VecNormalized = ignoreScale ? vecToConvert : vecToConvert * axisMag;

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y);
            Vector2 convertedVec = new Vector2(x, y);


            return convertedVec;
        }

        #endregion

        #endregion

        #region Position Type conversions

        #region WorldToLocal overrides

        /// <summary>
        /// override for ConvertPointFromSystemAToSystemB, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="posToConvert">point to convert, given in world axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="origin">origin of local axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <returns></returns>
        public static Vector2 PointWorldToLocal(Vector2 posToConvert, Vector2 origin, Vector2 xAxis, Vector2 yAxis)
        {
            return ConvertPointFromSystemAToSystemB(posToConvert, origin, xAxis, yAxis);
        }

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="posToConvert">vector to convert, given in world axis</param>
        /// <param name="axis">axis representing local x or y axis</param>
        /// <param name="origin">origin of local axis</param>
        /// <param name="axisToFind">is the vector above representing x or y ?</param>
        /// <returns></returns>
        public static Vector2 PointWorldToLocal(Vector2 posToConvert, Vector2 origin, Vector2 axis, Axis axisToFind)
        {
            return ConvertPointFromSystemAToSystemB(posToConvert, origin, axis, axisToFind);
        }

        /// <summary>
        /// override for ConvertPointFromSystemAToSystemB, basically just to remember wich method to use
        /// </summary>
        /// <param name="posToConvert">point to convert, given in world axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="origin">origin of local axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector2 PointWorldToLocal(Vector2 posToConvert, Vector2 origin, Vector2 xAxis, Vector2 yAxis, bool ignoreScale)
        {
            return ConvertPointFromSystemAToSystemB(posToConvert, origin, xAxis, yAxis, ignoreScale);
        }

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// </summary>
        /// <param name="posToConvert">vector to convert, given in world axis</param>
        /// <param name="axis">axis representing local x or y axis</param>
        /// <param name="origin">origin of local axis</param>
        /// <param name="axisToFind">is the vector above representing x or y ?</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector2 PointWorldToLocal(Vector2 posToConvert, Vector2 origin, Vector2 axis, Axis axisToFind, bool ignoreScale)
        {
            return ConvertPointFromSystemAToSystemB(posToConvert, origin, axis, axisToFind, ignoreScale);
        }

        #endregion

        #region localToWorld overrides

        /// <summary>
        /// override for ConvertPointToSystemBFromSystemA, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="posToConvert">point to convert, given in local axis</param>
        /// <param name="origin">origin of local axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <returns></returns>
        public static Vector2 PointLocalToWorld(Vector2 posToConvert, Vector2 origin, Vector2 xAxis, Vector2 yAxis)
        {
            return ConvertPointToSystemBFromSystemA(posToConvert, origin, xAxis, yAxis);
        }

        /// <summary>
        /// override for ConvertPointToSystemBFromSystemA, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="posToConvert">vector to convert, given in world axis</param>
        /// <param name="axis">axis representing local x or y axis</param>
        /// <param name="origin">origin of local axis</param>
        /// <param name="axisToFind">is the vector above representing x or y ?</param>
        /// <returns></returns>
        public static Vector2 PointLocalToWorld(Vector2 posToConvert, Vector2 origin, Vector2 axis, Axis axisToFind)
        {
            return ConvertPointToSystemBFromSystemA(posToConvert, origin, axis, axisToFind);
        }

        /// <summary>
        /// override for ConvertPointToSystemBFromSystemA, basically just to remember wich method to use
        /// </summary>
        /// <param name="posToConvert">point to convert, given in local axis</param>
        /// <param name="origin">origin of local axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector2 PointLocalToWorld(Vector2 posToConvert, Vector2 origin, Vector2 xAxis, Vector2 yAxis, bool ignoreScale)
        {
            return ConvertPointToSystemBFromSystemA(posToConvert, origin, xAxis, yAxis, ignoreScale);
        }

        /// <summary>
        /// override for ConvertPointToSystemBFromSystemA, basically just to remember wich method to use
        /// </summary>
        /// <param name="posToConvert">vector to convert, given in world axis</param>
        /// <param name="axis">axis representing local x or y axis</param>
        /// <param name="origin">origin of local axis</param>
        /// <param name="axisToFind">is the vector above representing x or y ?</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector2 PointLocalToWorld(Vector2 posToConvert, Vector2 origin, Vector2 axis, Axis axisToFind, bool ignoreScale)
        {
            return ConvertPointToSystemBFromSystemA(posToConvert, origin, axis, axisToFind, ignoreScale);
        }

        #endregion

        #region from A to B 

        /// <summary>
        /// convert a given point from coordinate system A to B
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="posToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxisOfB">axis representing x axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing y axis of system B, given in system A</param>
        /// <returns></returns>
        public static Vector2 ConvertPointFromSystemAToSystemB(Vector2 posToConvert, Vector2 origin, Vector2 xAxisOfB, Vector2 yAxisOfB)
        {
            Vector2 offsetedPod = posToConvert - origin;

            Vector2 xAxisNormalized = xAxisOfB.normalized;
            Vector2 yAxisNormalized = yAxisOfB.normalized;
            Vector2 axisMag = new Vector2(xAxisOfB.magnitude, yAxisOfB.magnitude);

            float x = (offsetedPod.x * xAxisNormalized.x) + (offsetedPod.y * xAxisNormalized.y);
            float y = (offsetedPod.x * yAxisNormalized.x) + (offsetedPod.y * yAxisNormalized.y);
            Vector2 convertedNormalizedVec = new Vector2(x, y);

            Vector2 convertedVec = convertedNormalizedVec / axisMag;

            return convertedVec;
        }

        /// <summary>
        /// convert a given point from coordinate system A to B
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="posToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxisOfB">axis representing x or y axis of system B, given in system A</param>
        /// <param name="axisToFind">is axis given above representing x or y ?</param>
        /// <returns></returns>
        public static Vector2 ConvertPointFromSystemAToSystemB(Vector2 posToConvert, Vector2 origin, Vector2 axisOfB, Axis axisToFind)
        {
            Vector2 xAxisOfB = Vector2.zero;
            Vector2 yAxisOfB = Vector2.zero;

            FindPerpToVec(axisOfB, axisToFind, ref xAxisOfB, ref yAxisOfB);

            Vector2 offsetedPod = posToConvert - origin;

            Vector2 xAxisNormalized = xAxisOfB.normalized;
            Vector2 yAxisNormalized = yAxisOfB.normalized;
            Vector2 axisMag = new Vector2(xAxisOfB.magnitude, yAxisOfB.magnitude);

            float x = (offsetedPod.x * xAxisNormalized.x) + (offsetedPod.y * xAxisNormalized.y);
            float y = (offsetedPod.x * yAxisNormalized.x) + (offsetedPod.y * yAxisNormalized.y);
            Vector2 convertedNormalizedVec = new Vector2(x, y);

            Vector2 convertedVec = convertedNormalizedVec / axisMag;

            return convertedVec;
        }

        /// <summary>
        /// convert a given point from coordinate system A to B
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="posToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxisOfB">axis representing x axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing y axis of system B, given in system A</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector2 ConvertPointFromSystemAToSystemB(Vector2 posToConvert, Vector2 origin, Vector2 xAxisOfB, Vector2 yAxisOfB, bool ignoreScale)
        {
            Vector2 offsetedPod = posToConvert - origin;

            Vector2 xAxisNormalized = xAxisOfB.normalized;
            Vector2 yAxisNormalized = yAxisOfB.normalized;
            Vector2 axisMag = new Vector2(xAxisOfB.magnitude, yAxisOfB.magnitude);

            float x = (offsetedPod.x * xAxisNormalized.x) + (offsetedPod.y * xAxisNormalized.y);
            float y = (offsetedPod.x * yAxisNormalized.x) + (offsetedPod.y * yAxisNormalized.y);
            Vector2 convertedNormalizedVec = new Vector2(x, y);

            Vector2 convertedVec = ignoreScale ? convertedNormalizedVec : convertedNormalizedVec / axisMag;

            return convertedVec;
        }

        /// <summary>
        /// convert a given point from coordinate system A to B
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="posToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxisOfB">axis representing x or y axis of system B, given in system A</param>
        /// <param name="axisToFind">is axis given above representing x or y ?</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector2 ConvertPointFromSystemAToSystemB(Vector2 posToConvert, Vector2 origin, Vector2 axisOfB, Axis axisToFind, bool ignoreScale)
        {
            Vector2 xAxisOfB = Vector2.zero;
            Vector2 yAxisOfB = Vector2.zero;

            FindPerpToVec(axisOfB, axisToFind, ref xAxisOfB, ref yAxisOfB);

            Vector2 offsetedPod = posToConvert - origin;

            Vector2 xAxisNormalized = xAxisOfB.normalized;
            Vector2 yAxisNormalized = yAxisOfB.normalized;
            Vector2 axisMag = new Vector2(xAxisOfB.magnitude, yAxisOfB.magnitude);

            float x = (offsetedPod.x * xAxisNormalized.x) + (offsetedPod.y * xAxisNormalized.y);
            float y = (offsetedPod.x * yAxisNormalized.x) + (offsetedPod.y * yAxisNormalized.y);
            Vector2 convertedNormalizedVec = new Vector2(x, y);

            Vector2 convertedVec = ignoreScale ? convertedNormalizedVec : convertedNormalizedVec / axisMag;

            return convertedVec;
        }

        #endregion

        #region to B from A

        /// <summary>
        /// convert a given point to coordinate system B from A
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="posToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system A, given in system B</param>
        /// <param name="xAxisOfA">axis representing x axis of system A, given in system B</param>
        /// <param name="yAxisOfA">axis representing y axis of system A, given in system B</param>
        /// <returns></returns>
        public static Vector2 ConvertPointToSystemBFromSystemA(Vector2 posToConvert, Vector2 origin, Vector2 xAxisOfA, Vector2 yAxisOfA)
        {
            Vector2 xAxisNormalized = xAxisOfA.normalized;
            Vector2 yAxisNormalized = yAxisOfA.normalized;
            Vector2 axisMag = new Vector2(xAxisOfA.magnitude, yAxisOfA.magnitude);

            Vector2 VecNormalized = posToConvert * axisMag;

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y);
            Vector2 convertedVec = new Vector2(x, y);


            return convertedVec + origin;
        }

        /// <summary>
        /// convert a given point to coordinate system B from A
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="posToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system A, given in system B</param>
        /// <param name="axisOfA">axis representing x or y axis of system A, given in system B</param>
        /// <param name="axisToFind">is axis given above representing x or y ?</param>
        /// <returns></returns>
        public static Vector2 ConvertPointToSystemBFromSystemA(Vector2 posToConvert, Vector2 origin, Vector2 axisOfA, Axis axisToFind)
        {
            Vector2 xAxisOfA = Vector2.zero;
            Vector2 yAxisOfA = Vector2.zero;

            FindPerpToVec(axisOfA, axisToFind, ref xAxisOfA, ref yAxisOfA);

            Vector2 xAxisNormalized = xAxisOfA.normalized;
            Vector2 yAxisNormalized = yAxisOfA.normalized;
            Vector2 axisMag = new Vector2(xAxisOfA.magnitude, yAxisOfA.magnitude);

            Vector2 VecNormalized = posToConvert * axisMag;

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y);
            Vector2 convertedVec = new Vector2(x, y);


            return convertedVec + origin;
        }

        /// <summary>
        /// convert a given point to coordinate system B from A
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="posToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system A, given in system B</param>
        /// <param name="xAxisOfA">axis representing x axis of system A, given in system B</param>
        /// <param name="yAxisOfA">axis representing y axis of system A, given in system B</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector2 ConvertPointToSystemBFromSystemA(Vector2 posToConvert, Vector2 origin, Vector2 xAxisOfA, Vector2 yAxisOfA, bool ignoreScale)
        {
            Vector2 xAxisNormalized = xAxisOfA.normalized;
            Vector2 yAxisNormalized = yAxisOfA.normalized;
            Vector2 axisMag = new Vector2(xAxisOfA.magnitude, yAxisOfA.magnitude);

            Vector2 VecNormalized = ignoreScale ? posToConvert : posToConvert * axisMag;

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y);
            Vector2 convertedVec = new Vector2(x, y);


            return convertedVec + origin;
        }

        /// <summary>
        /// convert a given point to coordinate system B from A
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="posToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system A, given in system B</param>
        /// <param name="axisOfA">axis representing x or y axis of system A, given in system B</param>
        /// <param name="axisToFind">is axis given above representing x or y ?</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector2 ConvertPointToSystemBFromSystemA(Vector2 posToConvert, Vector2 origin, Vector2 axisOfA, Axis axisToFind, bool ignoreScale)
        {
            Vector2 xAxisOfA = Vector2.zero;
            Vector2 yAxisOfA = Vector2.zero;

            FindPerpToVec(axisOfA, axisToFind, ref xAxisOfA, ref yAxisOfA);

            Vector2 xAxisNormalized = xAxisOfA.normalized;
            Vector2 yAxisNormalized = yAxisOfA.normalized;
            Vector2 axisMag = new Vector2(xAxisOfA.magnitude, yAxisOfA.magnitude);

            Vector2 VecNormalized = ignoreScale ? posToConvert : posToConvert * axisMag;

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y);
            Vector2 convertedVec = new Vector2(x, y);


            return convertedVec + origin;
        }

        #endregion

        #endregion

        #endregion


        #region Vector3CoordinateConversion

        #region Vector type conversions

        #region WorldToLocal overrides

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in world axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <returns></returns>
        public static Vector3 VecWorldToLocal(Vector3 vecToConvert, Vector3 xAxis, Vector3 yAxis, Vector3 zAxis)
        {
            return ConvertVectorFromSystemAToSystemB(vecToConvert, xAxis, yAxis, zAxis);
        }

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in world axis</param>
        /// <param name="axisOne">axis representing one local axis</param>
        /// <param name="axisTwo">axis representing another local axis</param>
        /// <param name="axisOneType">what first axis represent in local axis</param>
        /// <param name="axisTwoType">what second axis represent in local axis</param>
        /// <returns></returns>
        public static Vector3 VecWorldToLocal(Vector3 vecToConvert, Vector3 axisOne, Vector3 axisTwo, Axis axisOneType, Axis axisTwoType)
        {
            return ConvertVectorFromSystemAToSystemB(vecToConvert, axisOne, axisTwo, axisOneType, axisTwoType);
        }

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in world axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector3 VecWorldToLocal(Vector3 vecToConvert, Vector3 xAxis, Vector3 yAxis, Vector3 zAxis, bool ignoreScale)
        {
            return ConvertVectorFromSystemAToSystemB(vecToConvert, xAxis, yAxis, zAxis, ignoreScale);
        }

        /// <summary>
        /// override for ConvertVectorFromSystemAToSystemB, basically just to remember wich method to use
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in world axis</param>
        /// <param name="axisOne">axis representing one local axis</param>
        /// <param name="axisTwo">axis representing another local axis</param>
        /// <param name="axisOneType">what first axis represent in local axis</param>
        /// <param name="axisTwoType">what second axis represent in local axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector3 VecWorldToLocal(Vector3 vecToConvert, Vector3 axisOne, Vector3 axisTwo, Axis axisOneType, Axis axisTwoType, bool ignoreScale)
        {
            return ConvertVectorFromSystemAToSystemB(vecToConvert, axisOne, axisTwo, axisOneType, axisTwoType, ignoreScale);
        }

        #endregion

        #region localToWorld overrides

        /// <summary>
        /// override for ConvertVectorToSystemBFromSystemA, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in local axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <param name="zAxis">axis representing local z axis</param>
        /// <returns></returns>
        public static Vector3 VecLocalToWorld(Vector3 vecToConvert, Vector3 xAxis, Vector3 yAxis, Vector3 zAxis)
        {
            return ConvertVectorToSystemBFromSystemA(vecToConvert, xAxis, yAxis, zAxis);
        }

        /// <summary>
        /// override for ConvertVectorToSystemBFromSystemA, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in local axis</param>
        /// <param name="axisOne">axis representing one local axis</param>
        /// <param name="axisTwo">axis representing another local axis</param>
        /// <param name="axisOneType">what first axis represent in local axis</param>
        /// <param name="axisTwoType">what second axis represent in local axis</param>
        /// <returns></returns>
        public static Vector3 VecLocalToWorld(Vector3 vecToConvert, Vector3 axisOne, Vector3 axisTwo, Axis axisOneType, Axis axisTwoType)
        {
            return ConvertVectorToSystemBFromSystemA(vecToConvert, axisOne, axisTwo, axisOneType, axisTwoType);
        }

        /// <summary>
        /// override for ConvertVectorToSystemBFromSystemA, basically just to remember wich method to use
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in local axis</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <param name="zAxis">axis representing local z axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector3 VecLocalToWorld(Vector3 vecToConvert, Vector3 xAxis, Vector3 yAxis, Vector3 zAxis, bool ignoreScale)
        {
            return ConvertVectorToSystemBFromSystemA(vecToConvert, xAxis, yAxis, zAxis, ignoreScale);
        }

        /// <summary>
        /// override for ConvertVectorToSystemBFromSystemA, basically just to remember wich method to use
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in local axis</param>
        /// <param name="axisOne">axis representing one local axis</param>
        /// <param name="axisTwo">axis representing another local axis</param>
        /// <param name="axisOneType">what first axis represent in local axis</param>
        /// <param name="axisTwoType">what second axis represent in local axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector3 VecLocalToWorld(Vector3 vecToConvert, Vector3 axisOne, Vector3 axisTwo, Axis axisOneType, Axis axisTwoType, bool ignoreScale)
        {
            return ConvertVectorToSystemBFromSystemA(vecToConvert, axisOne, axisTwo, axisOneType, axisTwoType, ignoreScale);
        }

        #endregion

        #region from A to B 

        /// <summary>
        /// convert a given vector from coordinate system A to B
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="xAxisOfB">axis representing x axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing y axis of system B, given in system A</param>
        /// <param name="zAxisOfB">axis representing z axis of system B, given in system A</param>
        /// <returns></returns>
        public static Vector3 ConvertVectorFromSystemAToSystemB(Vector3 vecToConvert, Vector3 xAxisOfB, Vector3 yAxisOfB, Vector3 zAxisOfB)
        {
            Vector3 xAxisNormalized = xAxisOfB.normalized;
            Vector3 yAxisNormalized = yAxisOfB.normalized;
            Vector3 zAxisNormalized = zAxisOfB.normalized;

            Vector3 axisMag = new Vector3(xAxisOfB.magnitude, yAxisOfB.magnitude, zAxisOfB.magnitude);

            float x = (vecToConvert.x * xAxisNormalized.x) + (vecToConvert.y * xAxisNormalized.y) + (vecToConvert.z * xAxisNormalized.z);
            float y = (vecToConvert.x * yAxisNormalized.x) + (vecToConvert.y * yAxisNormalized.y) + (vecToConvert.z * yAxisNormalized.z);
            float z = (vecToConvert.x * zAxisNormalized.x) + (vecToConvert.y * zAxisNormalized.y) + (vecToConvert.z * zAxisNormalized.z);

            Vector3 convertedNormalizedVec = new Vector3(x, y, z);

            Vector3 convertedVec = UPDBMath.VecDivide(convertedNormalizedVec, axisMag);

            return convertedVec;
        }

        /// <summary>
        /// convert a given vector from coordinate system A to B
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="xAxisOfB">axis representing an axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing an axis of system B, given in system A</param>
        /// <param name="firstAxis">first axis given</param>
        /// <param name="secondAxis">second axis given</param>
        /// <returns></returns>
        public static Vector3 ConvertVectorFromSystemAToSystemB(Vector3 vecToConvert, Vector3 axisOneOfB, Vector3 axisTwoOfB, Axis firstAxis, Axis secondAxis)
        {
            Vector3 xAxisOfB = Vector3.zero;
            Vector3 yAxisOfB = Vector3.zero;
            Vector3 zAxisOfB = Vector3.zero;

            FindPerpToVecs(axisOneOfB, axisTwoOfB, firstAxis, secondAxis, ref xAxisOfB, ref yAxisOfB, ref zAxisOfB);

            Vector3 xAxisNormalized = xAxisOfB.normalized;
            Vector3 yAxisNormalized = yAxisOfB.normalized;
            Vector3 zAxisNormalized = zAxisOfB.normalized;

            Vector3 axisMag = new Vector3(xAxisOfB.magnitude, yAxisOfB.magnitude, zAxisOfB.magnitude);

            float x = (vecToConvert.x * xAxisNormalized.x) + (vecToConvert.y * xAxisNormalized.y) + (vecToConvert.z * xAxisNormalized.z);
            float y = (vecToConvert.x * yAxisNormalized.x) + (vecToConvert.y * yAxisNormalized.y) + (vecToConvert.z * yAxisNormalized.z);
            float z = (vecToConvert.x * zAxisNormalized.x) + (vecToConvert.y * zAxisNormalized.y) + (vecToConvert.z * zAxisNormalized.z);

            Vector3 convertedNormalizedVec = new Vector3(x, y, z);

            Vector3 convertedVec = UPDBMath.VecDivide(convertedNormalizedVec, axisMag);

            return convertedVec;
        }

        //EXPERIMENTAL
        /// <summary>
        /// convert a given vector from coordinate system A to B
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="xAxisOfB">axis representing an axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing an axis of system B, given in system A</param>
        /// <param name="firstAxis">first axis given</param>
        /// <param name="secondAxis">second axis given</param>
        /// <returns></returns>
        public static Vector3 ConvertVectorFromSystemAToSystemB(Vector3 vecToConvert, Vector3 axisOfB, Axis givenAxis)
        {
            Vector3 xAxisOfB = Vector3.zero;
            Vector3 yAxisOfB = Vector3.zero;
            Vector3 zAxisOfB = Vector3.zero;

            //Vector3 originalAxisOfB = axisOfB;
            //axisOfB = new Vector3(Mathf.Abs(axisOfB.x), Mathf.Abs(axisOfB.y), Mathf.Abs(axisOfB.z));
            Vector3 axisTwoOfB = UPDBMath.FindAnyPerpendicularVectorUpType(axisOfB);

            if (givenAxis == Axis.X)
            {
                FindPerpToVecs(axisOfB, axisTwoOfB, Axis.X, Axis.Y, ref xAxisOfB, ref yAxisOfB, ref zAxisOfB);
            }
            if(givenAxis == Axis.Y)
            {
                FindPerpToVecs(axisOfB, axisTwoOfB, Axis.Y, Axis.X, ref xAxisOfB, ref yAxisOfB, ref zAxisOfB);

            }
            if(givenAxis == Axis.Z)
            {
                FindPerpToVecs(axisOfB, axisTwoOfB, Axis.Z, Axis.Y, ref xAxisOfB, ref yAxisOfB, ref zAxisOfB);

            }


            Debug.DrawRay(Vector3.zero, xAxisOfB);
            Debug.DrawRay(Vector3.zero, yAxisOfB);
            //Debug.DrawRay(Vector3.zero, zAxisOfB);
            
            Vector3 xAxisNormalized = xAxisOfB.normalized;
            Vector3 yAxisNormalized = yAxisOfB.normalized;
            Vector3 zAxisNormalized = zAxisOfB.normalized;

            Vector3 axisMag = new Vector3(xAxisOfB.magnitude, yAxisOfB.magnitude, zAxisOfB.magnitude);

            float x = (vecToConvert.x * xAxisNormalized.x) + (vecToConvert.y * xAxisNormalized.y) + (vecToConvert.z * xAxisNormalized.z);
            float y = (vecToConvert.x * yAxisNormalized.x) + (vecToConvert.y * yAxisNormalized.y) + (vecToConvert.z * yAxisNormalized.z);
            float z = (vecToConvert.x * zAxisNormalized.x) + (vecToConvert.y * zAxisNormalized.y) + (vecToConvert.z * zAxisNormalized.z);

            Vector3 convertedNormalizedVec = new Vector3(x, y, z);

            Vector3 convertedVec = UPDBMath.VecDivide(convertedNormalizedVec, axisMag);

            return convertedVec;
        }

        /// <summary>
        /// convert a given vector from coordinate system A to B
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="xAxisOfB">axis representing x axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing y axis of system B, given in system A</param>
        /// <param name="zAxisOfB">axis representing z axis of system B, given in system A</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector3 ConvertVectorFromSystemAToSystemB(Vector3 vecToConvert, Vector3 xAxisOfB, Vector3 yAxisOfB, Vector3 zAxisOfB, bool ignoreScale)
        {
            Vector3 xAxisNormalized = xAxisOfB.normalized;
            Vector3 yAxisNormalized = yAxisOfB.normalized;
            Vector3 zAxisNormalized = zAxisOfB.normalized;

            Vector3 axisMag = new Vector3(xAxisOfB.magnitude, yAxisOfB.magnitude, zAxisOfB.magnitude);

            float x = (vecToConvert.x * xAxisNormalized.x) + (vecToConvert.y * xAxisNormalized.y) + (vecToConvert.z * xAxisNormalized.z);
            float y = (vecToConvert.x * yAxisNormalized.x) + (vecToConvert.y * yAxisNormalized.y) + (vecToConvert.z * yAxisNormalized.z);
            float z = (vecToConvert.x * zAxisNormalized.x) + (vecToConvert.y * zAxisNormalized.y) + (vecToConvert.z * zAxisNormalized.z);

            Vector3 convertedNormalizedVec = new Vector3(x, y, z);

            Vector3 convertedVec = ignoreScale ? convertedNormalizedVec : UPDBMath.VecDivide(convertedNormalizedVec, axisMag);

            return convertedVec;
        }

        /// <summary>
        /// convert a given vector from coordinate system A to B
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="xAxisOfB">axis representing an axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing an axis of system B, given in system A</param>
        /// <param name="firstAxis">first axis given</param>
        /// <param name="secondAxis">second axis given</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector3 ConvertVectorFromSystemAToSystemB(Vector3 vecToConvert, Vector3 axisOneOfB, Vector3 axisTwoOfB, Axis firstAxis, Axis secondAxis, bool ignoreScale)
        {
            Vector3 xAxisOfB = Vector3.zero;
            Vector3 yAxisOfB = Vector3.zero;
            Vector3 zAxisOfB = Vector3.zero;

            FindPerpToVecs(axisOneOfB, axisTwoOfB, firstAxis, secondAxis, ref xAxisOfB, ref yAxisOfB, ref zAxisOfB);

            Vector3 xAxisNormalized = xAxisOfB.normalized;
            Vector3 yAxisNormalized = yAxisOfB.normalized;
            Vector3 zAxisNormalized = zAxisOfB.normalized;

            Vector3 axisMag = new Vector3(xAxisOfB.magnitude, yAxisOfB.magnitude, zAxisOfB.magnitude);

            float x = (vecToConvert.x * xAxisNormalized.x) + (vecToConvert.y * xAxisNormalized.y) + (vecToConvert.z * xAxisNormalized.z);
            float y = (vecToConvert.x * yAxisNormalized.x) + (vecToConvert.y * yAxisNormalized.y) + (vecToConvert.z * yAxisNormalized.z);
            float z = (vecToConvert.x * zAxisNormalized.x) + (vecToConvert.y * zAxisNormalized.y) + (vecToConvert.z * zAxisNormalized.z);

            Vector3 convertedNormalizedVec = new Vector3(x, y, z);

            Vector3 convertedVec = ignoreScale ? convertedNormalizedVec : UPDBMath.VecDivide(convertedNormalizedVec, axisMag);

            return convertedVec;
        }

        #endregion

        #region to B from A

        /// <summary>
        /// convert a given vector to coordinate system B from A
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="xAxisOfA">axis representing x axis of system A, given in system B</param>
        /// <param name="yAxisOfA">axis representing y axis of system A, given in system B</param>
        /// <param name="zAxisOfA">axis representing z axis of system A, given in system B</param>
        /// <returns></returns>
        public static Vector3 ConvertVectorToSystemBFromSystemA(Vector3 vecToConvert, Vector3 xAxisOfA, Vector3 yAxisOfA, Vector3 zAxisOfA)
        {
            Vector3 xAxisNormalized = xAxisOfA.normalized;
            Vector3 yAxisNormalized = yAxisOfA.normalized;
            Vector3 zAxisNormalized = zAxisOfA.normalized;
            Vector3 axisMag = new Vector3(xAxisOfA.magnitude, yAxisOfA.magnitude, zAxisOfA.magnitude);

            Vector3 VecNormalized = UPDBMath.VecTime(vecToConvert, axisMag);

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x) + (VecNormalized.z * zAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y) + (VecNormalized.z * zAxisNormalized.y);
            float z = (VecNormalized.x * xAxisNormalized.z) + (VecNormalized.y * yAxisNormalized.z) + (VecNormalized.z * zAxisNormalized.z);

            Vector3 convertedVec = new Vector3(x, y, z);

            return convertedVec;
        }

        /// <summary>
        /// convert a given vector to coordinate system B from A
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="axisOneOfA">axis representing an axis of system A, given in system B</param>
        /// <param name="axisTwoOfA">axis representing an axis of system A, given in system B</param>
        /// <param name="firstAxis">first axis given</param>
        /// <param name="secondAxis">second axis given</param>
        /// <returns></returns>
        public static Vector3 ConvertVectorToSystemBFromSystemA(Vector3 vecToConvert, Vector3 axisOneOfA, Vector3 axisTwoOfA, Axis firstAxis, Axis secondAxis)
        {
            Vector3 xAxisOfA = Vector3.zero;
            Vector3 yAxisOfA = Vector3.zero;
            Vector3 zAxisOfA = Vector3.zero;

            FindPerpToVecs(axisOneOfA, axisTwoOfA, firstAxis, secondAxis, ref xAxisOfA, ref yAxisOfA, ref zAxisOfA);

            Vector3 xAxisNormalized = xAxisOfA.normalized;
            Vector3 yAxisNormalized = yAxisOfA.normalized;
            Vector3 zAxisNormalized = zAxisOfA.normalized;
            Vector3 axisMag = new Vector3(xAxisOfA.magnitude, yAxisOfA.magnitude, zAxisOfA.magnitude);

            Vector3 VecNormalized = UPDBMath.VecTime(vecToConvert, axisMag);

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x) + (VecNormalized.z * zAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y) + (VecNormalized.z * zAxisNormalized.y);
            float z = (VecNormalized.x * xAxisNormalized.z) + (VecNormalized.y * yAxisNormalized.z) + (VecNormalized.z * zAxisNormalized.z);

            Vector3 convertedVec = new Vector3(x, y, z);

            return convertedVec;
        }

        /// <summary>
        /// convert a given vector to coordinate system B from A
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="xAxisOfA">axis representing x axis of system A, given in system B</param>
        /// <param name="yAxisOfA">axis representing y axis of system A, given in system B</param>
        /// <param name="zAxisOfA">axis representing z axis of system A, given in system B</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector3 ConvertVectorToSystemBFromSystemA(Vector3 vecToConvert, Vector3 xAxisOfA, Vector3 yAxisOfA, Vector3 zAxisOfA, bool ignoreScale)
        {
            Vector3 xAxisNormalized = xAxisOfA.normalized;
            Vector3 yAxisNormalized = yAxisOfA.normalized;
            Vector3 zAxisNormalized = zAxisOfA.normalized;
            Vector3 axisMag = new Vector3(xAxisOfA.magnitude, yAxisOfA.magnitude, zAxisOfA.magnitude);

            Vector3 VecNormalized = ignoreScale ? vecToConvert : UPDBMath.VecTime(vecToConvert, axisMag);

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x) + (VecNormalized.z * zAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y) + (VecNormalized.z * zAxisNormalized.y);
            float z = (VecNormalized.x * xAxisNormalized.z) + (VecNormalized.y * yAxisNormalized.z) + (VecNormalized.z * zAxisNormalized.z);

            Vector3 convertedVec = new Vector3(x, y, z);

            return convertedVec;
        }

        /// <summary>
        /// convert a given vector to coordinate system B from A
        /// note 1 : vector type of conversion ignore the position, if you enter a point, it will consider it as a vector starting from origin of system and going to this position
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="vecToConvert">vector to convert, given in system A</param>
        /// <param name="axisOneOfA">axis representing an axis of system A, given in system B</param>
        /// <param name="axisTwoOfA">axis representing an axis of system A, given in system B</param>
        /// <param name="firstAxis">first axis given</param>
        /// <param name="secondAxis">second axis given</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector3 ConvertVectorToSystemBFromSystemA(Vector3 vecToConvert, Vector3 axisOneOfA, Vector3 axisTwoOfA, Axis firstAxis, Axis secondAxis, bool ignoreScale)
        {
            Vector3 xAxisOfA = Vector3.zero;
            Vector3 yAxisOfA = Vector3.zero;
            Vector3 zAxisOfA = Vector3.zero;

            FindPerpToVecs(axisOneOfA, axisTwoOfA, firstAxis, secondAxis, ref xAxisOfA, ref yAxisOfA, ref zAxisOfA);

            Vector3 xAxisNormalized = xAxisOfA.normalized;
            Vector3 yAxisNormalized = yAxisOfA.normalized;
            Vector3 zAxisNormalized = zAxisOfA.normalized;
            Vector3 axisMag = new Vector3(xAxisOfA.magnitude, yAxisOfA.magnitude, zAxisOfA.magnitude);

            Vector3 VecNormalized = ignoreScale ? vecToConvert : UPDBMath.VecTime(vecToConvert, axisMag);

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x) + (VecNormalized.z * zAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y) + (VecNormalized.z * zAxisNormalized.y);
            float z = (VecNormalized.x * xAxisNormalized.z) + (VecNormalized.y * yAxisNormalized.z) + (VecNormalized.z * zAxisNormalized.z);

            Vector3 convertedVec = new Vector3(x, y, z);

            return convertedVec;
        }

        #endregion

        #endregion

        #region Position Type conversions

        #region WorldToLocal overrides

        /// <summary>
        /// override for ConvertPointFromSystemAToSystemB, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in world axis</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <returns></returns>
        public static Vector3 PointWorldToLocal(Vector3 pointToConvert, Vector3 origin, Vector3 xAxis, Vector3 yAxis, Vector3 zAxis)
        {
            return ConvertPointFromSystemAToSystemB(pointToConvert, origin, xAxis, yAxis, zAxis);
        }

        /// <summary>
        /// override for ConvertPointFromSystemAToSystemB, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in world axis</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="axisOne">axis representing one local axis</param>
        /// <param name="axisTwo">axis representing another local axis</param>
        /// <param name="axisOneType">what first axis represent in local axis</param>
        /// <param name="axisTwoType">what second axis represent in local axis</param>
        /// <returns></returns>
        public static Vector3 PointWorldToLocal(Vector3 pointToConvert, Vector3 origin, Vector3 axisOne, Vector3 axisTwo, Axis axisOneType, Axis axisTwoType)
        {
            return ConvertPointFromSystemAToSystemB(pointToConvert, origin, axisOne, axisTwo, axisOneType, axisTwoType);
        }

        /// <summary>
        /// override for ConvertPointFromSystemAToSystemB, basically just to remember wich method to use
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in world axis</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector3 PointWorldToLocal(Vector3 pointToConvert, Vector3 origin, Vector3 xAxis, Vector3 yAxis, Vector3 zAxis, bool ignoreScale)
        {
            return ConvertPointFromSystemAToSystemB(pointToConvert, origin, xAxis, yAxis, zAxis, ignoreScale);
        }

        /// <summary>
        /// override for ConvertPointFromSystemAToSystemB, basically just to remember wich method to use
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in world axis</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="axisOne">axis representing one local axis</param>
        /// <param name="axisTwo">axis representing another local axis</param>
        /// <param name="axisOneType">what first axis represent in local axis</param>
        /// <param name="axisTwoType">what second axis represent in local axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector3 PointWorldToLocal(Vector3 pointToConvert, Vector3 origin, Vector3 axisOne, Vector3 axisTwo, Axis axisOneType, Axis axisTwoType, bool ignoreScale)
        {
            return ConvertPointFromSystemAToSystemB(pointToConvert, origin, axisOne, axisTwo, axisOneType, axisTwoType, ignoreScale);
        }

        #endregion

        #region localToWorld overrides

        /// <summary>
        /// override for ConvertPointToSystemBFromSystemA, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in local axis</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <param name="zAxis">axis representing local z axis</param>
        /// <returns></returns>
        public static Vector3 PointLocalToWorld(Vector3 pointToConvert, Vector3 origin, Vector3 xAxis, Vector3 yAxis, Vector3 zAxis)
        {
            return ConvertPointToSystemBFromSystemA(pointToConvert, origin, xAxis, yAxis, zAxis);
        }

        /// <summary>
        /// override for ConvertPointToSystemBFromSystemA, basically just to remember wich method to use
        /// Note : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in local axis</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="axisOne">axis representing one local axis</param>
        /// <param name="axisTwo">axis representing another local axis</param>
        /// <param name="axisOneType">what first axis represent in local axis</param>
        /// <param name="axisTwoType">what second axis represent in local axis</param>
        /// <returns></returns>
        public static Vector3 PointLocalToWorld(Vector3 pointToConvert, Vector3 origin, Vector3 axisOne, Vector3 axisTwo, Axis axisOneType, Axis axisTwoType)
        {
            return ConvertPointToSystemBFromSystemA(pointToConvert, origin, axisOne, axisTwo, axisOneType, axisTwoType);
        }

        /// <summary>
        /// override for ConvertPointToSystemBFromSystemA, basically just to remember wich method to use
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in local axis</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxis">axis representing local x axis</param>
        /// <param name="yAxis">axis representing local y axis</param>
        /// <param name="zAxis">axis representing local z axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector3 PointLocalToWorld(Vector3 pointToConvert, Vector3 origin, Vector3 xAxis, Vector3 yAxis, Vector3 zAxis, bool ignoreScale)
        {
            return ConvertPointToSystemBFromSystemA(pointToConvert, origin, xAxis, yAxis, zAxis, ignoreScale);
        }

        /// <summary>
        /// override for ConvertPointToSystemBFromSystemA, basically just to remember wich method to use
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in local axis</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="axisOne">axis representing one local axis</param>
        /// <param name="axisTwo">axis representing another local axis</param>
        /// <param name="axisOneType">what first axis represent in local axis</param>
        /// <param name="axisTwoType">what second axis represent in local axis</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector3 PointLocalToWorld(Vector3 pointToConvert, Vector3 origin, Vector3 axisOne, Vector3 axisTwo, Axis axisOneType, Axis axisTwoType, bool ignoreScale)
        {
            return ConvertPointToSystemBFromSystemA(pointToConvert, origin, axisOne, axisTwo, axisOneType, axisTwoType, ignoreScale);
        }

        #endregion

        #region from A to B 

        /// <summary>
        /// convert a given point from coordinate system A to B
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxisOfB">axis representing x axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing y axis of system B, given in system A</param>
        /// <param name="zAxisOfB">axis representing z axis of system B, given in system A</param>
        /// <returns></returns>
        public static Vector3 ConvertPointFromSystemAToSystemB(Vector3 pointToConvert, Vector3 origin, Vector3 xAxisOfB, Vector3 yAxisOfB, Vector3 zAxisOfB)
        {
            Vector3 offsetedPoint = pointToConvert - origin;

            Vector3 xAxisNormalized = xAxisOfB.normalized;
            Vector3 yAxisNormalized = yAxisOfB.normalized;
            Vector3 zAxisNormalized = zAxisOfB.normalized;

            Vector3 axisMag = new Vector3(xAxisOfB.magnitude, yAxisOfB.magnitude, zAxisOfB.magnitude);

            float x = (offsetedPoint.x * xAxisNormalized.x) + (offsetedPoint.y * xAxisNormalized.y) + (offsetedPoint.z * xAxisNormalized.z);
            float y = (offsetedPoint.x * yAxisNormalized.x) + (offsetedPoint.y * yAxisNormalized.y) + (offsetedPoint.z * yAxisNormalized.z);
            float z = (offsetedPoint.x * zAxisNormalized.x) + (offsetedPoint.y * zAxisNormalized.y) + (offsetedPoint.z * zAxisNormalized.z);

            Vector3 convertedNormalizedVec = new Vector3(x, y, z);

            Vector3 convertedVec = UPDBMath.VecDivide(convertedNormalizedVec, axisMag);

            return convertedVec;
        }

        /// <summary>
        /// convert a given point from coordinate system A to B
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxisOfB">axis representing an axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing an axis of system B, given in system A</param>
        /// <param name="firstAxis">first axis given</param>
        /// <param name="secondAxis">second axis given</param>
        /// <returns></returns>
        public static Vector3 ConvertPointFromSystemAToSystemB(Vector3 pointToConvert, Vector3 origin, Vector3 axisOneOfB, Vector3 axisTwoOfB, Axis firstAxis, Axis secondAxis)
        {
            Vector3 offsetedPoint = pointToConvert - origin;

            Vector3 xAxisOfB = Vector3.zero;
            Vector3 yAxisOfB = Vector3.zero;
            Vector3 zAxisOfB = Vector3.zero;

            FindPerpToVecs(axisOneOfB, axisTwoOfB, firstAxis, secondAxis, ref xAxisOfB, ref yAxisOfB, ref zAxisOfB);

            Vector3 xAxisNormalized = xAxisOfB.normalized;
            Vector3 yAxisNormalized = yAxisOfB.normalized;
            Vector3 zAxisNormalized = zAxisOfB.normalized;

            Vector3 axisMag = new Vector3(xAxisOfB.magnitude, yAxisOfB.magnitude, zAxisOfB.magnitude);

            float x = (offsetedPoint.x * xAxisNormalized.x) + (offsetedPoint.y * xAxisNormalized.y) + (offsetedPoint.z * xAxisNormalized.z);
            float y = (offsetedPoint.x * yAxisNormalized.x) + (offsetedPoint.y * yAxisNormalized.y) + (offsetedPoint.z * yAxisNormalized.z);
            float z = (offsetedPoint.x * zAxisNormalized.x) + (offsetedPoint.y * zAxisNormalized.y) + (offsetedPoint.z * zAxisNormalized.z);

            Vector3 convertedNormalizedVec = new Vector3(x, y, z);

            Vector3 convertedVec = UPDBMath.VecDivide(convertedNormalizedVec, axisMag);

            return convertedVec;
        }

        /// <summary>
        /// convert a given point from coordinate system A to B
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxisOfB">axis representing x axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing y axis of system B, given in system A</param>
        /// <param name="zAxisOfB">axis representing z axis of system B, given in system A</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector3 ConvertPointFromSystemAToSystemB(Vector3 pointToConvert, Vector3 origin, Vector3 xAxisOfB, Vector3 yAxisOfB, Vector3 zAxisOfB, bool ignoreScale)
        {
            Vector3 offsetedPoint = pointToConvert - origin;

            Vector3 xAxisNormalized = xAxisOfB.normalized;
            Vector3 yAxisNormalized = yAxisOfB.normalized;
            Vector3 zAxisNormalized = zAxisOfB.normalized;

            Vector3 axisMag = new Vector3(xAxisOfB.magnitude, yAxisOfB.magnitude, zAxisOfB.magnitude);

            float x = (offsetedPoint.x * xAxisNormalized.x) + (offsetedPoint.y * xAxisNormalized.y) + (offsetedPoint.z * xAxisNormalized.z);
            float y = (offsetedPoint.x * yAxisNormalized.x) + (offsetedPoint.y * yAxisNormalized.y) + (offsetedPoint.z * yAxisNormalized.z);
            float z = (offsetedPoint.x * zAxisNormalized.x) + (offsetedPoint.y * zAxisNormalized.y) + (offsetedPoint.z * zAxisNormalized.z);

            Vector3 convertedNormalizedVec = new Vector3(x, y, z);

            Vector3 convertedVec = ignoreScale ? convertedNormalizedVec : UPDBMath.VecDivide(convertedNormalizedVec, axisMag);

            return convertedVec;
        }

        /// <summary>
        /// convert a given point from coordinate system A to B
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxisOfB">axis representing an axis of system B, given in system A</param>
        /// <param name="yAxisOfB">axis representing an axis of system B, given in system A</param>
        /// <param name="firstAxis">first axis given</param>
        /// <param name="secondAxis">second axis given</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector3 ConvertPointFromSystemAToSystemB(Vector3 pointToConvert, Vector3 origin, Vector3 axisOneOfB, Vector3 axisTwoOfB, Axis firstAxis, Axis secondAxis, bool ignoreScale)
        {
            Vector3 offsetedPoint = pointToConvert - origin;

            Vector3 xAxisOfB = Vector3.zero;
            Vector3 yAxisOfB = Vector3.zero;
            Vector3 zAxisOfB = Vector3.zero;

            FindPerpToVecs(axisOneOfB, axisTwoOfB, firstAxis, secondAxis, ref xAxisOfB, ref yAxisOfB, ref zAxisOfB);

            Vector3 xAxisNormalized = xAxisOfB.normalized;
            Vector3 yAxisNormalized = yAxisOfB.normalized;
            Vector3 zAxisNormalized = zAxisOfB.normalized;

            Vector3 axisMag = new Vector3(xAxisOfB.magnitude, yAxisOfB.magnitude, zAxisOfB.magnitude);

            float x = (offsetedPoint.x * xAxisNormalized.x) + (offsetedPoint.y * xAxisNormalized.y) + (offsetedPoint.z * xAxisNormalized.z);
            float y = (offsetedPoint.x * yAxisNormalized.x) + (offsetedPoint.y * yAxisNormalized.y) + (offsetedPoint.z * yAxisNormalized.z);
            float z = (offsetedPoint.x * zAxisNormalized.x) + (offsetedPoint.y * zAxisNormalized.y) + (offsetedPoint.z * zAxisNormalized.z);

            Vector3 convertedNormalizedVec = new Vector3(x, y, z);

            Vector3 convertedVec = ignoreScale ? convertedNormalizedVec : UPDBMath.VecDivide(convertedNormalizedVec, axisMag);

            return convertedVec;
        }

        #endregion

        #region to B from A

        /// <summary>
        /// convert a given point to coordinate system B from A
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxisOfA">axis representing x axis of system A, given in system B</param>
        /// <param name="yAxisOfA">axis representing y axis of system A, given in system B</param>
        /// <param name="zAxisOfA">axis representing z axis of system A, given in system B</param>
        /// <returns></returns>
        public static Vector3 ConvertPointToSystemBFromSystemA(Vector3 pointToConvert, Vector3 origin, Vector3 xAxisOfA, Vector3 yAxisOfA, Vector3 zAxisOfA)
        {
            Vector3 xAxisNormalized = xAxisOfA.normalized;
            Vector3 yAxisNormalized = yAxisOfA.normalized;
            Vector3 zAxisNormalized = zAxisOfA.normalized;
            Vector3 axisMag = new Vector3(xAxisOfA.magnitude, yAxisOfA.magnitude, zAxisOfA.magnitude);

            Vector3 VecNormalized = UPDBMath.VecTime(pointToConvert, axisMag);

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x) + (VecNormalized.z * zAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y) + (VecNormalized.z * zAxisNormalized.y);
            float z = (VecNormalized.x * xAxisNormalized.z) + (VecNormalized.y * yAxisNormalized.z) + (VecNormalized.z * zAxisNormalized.z);

            Vector3 convertedVec = new Vector3(x, y, z);

            return convertedVec + origin;
        }

        /// <summary>
        /// convert a given point to coordinate system B from A
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// note 4 : by default, take in count scale of given vectors
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="axisOneOfA">axis representing an axis of system A, given in system B</param>
        /// <param name="axisTwoOfA">axis representing an axis of system A, given in system B</param>
        /// <param name="firstAxis">first axis given</param>
        /// <param name="secondAxis">second axis given</param>
        /// <returns></returns>
        public static Vector3 ConvertPointToSystemBFromSystemA(Vector3 pointToConvert, Vector3 origin, Vector3 axisOneOfA, Vector3 axisTwoOfA, Axis firstAxis, Axis secondAxis)
        {
            Vector3 xAxisOfA = Vector3.zero;
            Vector3 yAxisOfA = Vector3.zero;
            Vector3 zAxisOfA = Vector3.zero;

            FindPerpToVecs(axisOneOfA, axisTwoOfA, firstAxis, secondAxis, ref xAxisOfA, ref yAxisOfA, ref zAxisOfA);

            Vector3 xAxisNormalized = xAxisOfA.normalized;
            Vector3 yAxisNormalized = yAxisOfA.normalized;
            Vector3 zAxisNormalized = zAxisOfA.normalized;
            Vector3 axisMag = new Vector3(xAxisOfA.magnitude, yAxisOfA.magnitude, zAxisOfA.magnitude);

            Vector3 VecNormalized = UPDBMath.VecTime(pointToConvert, axisMag);

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x) + (VecNormalized.z * zAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y) + (VecNormalized.z * zAxisNormalized.y);
            float z = (VecNormalized.x * xAxisNormalized.z) + (VecNormalized.y * yAxisNormalized.z) + (VecNormalized.z * zAxisNormalized.z);

            Vector3 convertedVec = new Vector3(x, y, z);

            return convertedVec + origin;
        }

        /// <summary>
        /// convert a given point to coordinate system B from A
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="xAxisOfA">axis representing x axis of system A, given in system B</param>
        /// <param name="yAxisOfA">axis representing y axis of system A, given in system B</param>
        /// <param name="zAxisOfA">axis representing z axis of system A, given in system B</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector3 ConvertPointToSystemBFromSystemA(Vector3 pointToConvert, Vector3 origin, Vector3 xAxisOfA, Vector3 yAxisOfA, Vector3 zAxisOfA, bool ignoreScale)
        {
            Vector3 xAxisNormalized = xAxisOfA.normalized;
            Vector3 yAxisNormalized = yAxisOfA.normalized;
            Vector3 zAxisNormalized = zAxisOfA.normalized;
            Vector3 axisMag = new Vector3(xAxisOfA.magnitude, yAxisOfA.magnitude, zAxisOfA.magnitude);

            Vector3 VecNormalized = ignoreScale ? pointToConvert : UPDBMath.VecTime(pointToConvert, axisMag);

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x) + (VecNormalized.z * zAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y) + (VecNormalized.z * zAxisNormalized.y);
            float z = (VecNormalized.x * xAxisNormalized.z) + (VecNormalized.y * yAxisNormalized.z) + (VecNormalized.z * zAxisNormalized.z);

            Vector3 convertedVec = new Vector3(x, y, z);

            return convertedVec + origin;
        }

        /// <summary>
        /// convert a given point to coordinate system B from A
        /// note 1 : point type of conversion offset the position, if you enter a vector, it will consider it as a point, and your vector is gonna stay to the same final point, but it's origin will change(relatively to coords origin, this as nonsense otherwise)
        /// note 2 : "to" type conversion need the x and y vec representing the system B given in system A coords
        /// note 3 : "from" type conversion need the x and y vec representing the system A given in system B coords
        /// </summary>
        /// <param name="pointToConvert">point to convert, given in system A</param>
        /// <param name="origin">origin of system B, given in system A</param>
        /// <param name="axisOneOfA">axis representing an axis of system A, given in system B</param>
        /// <param name="axisTwoOfA">axis representing an axis of system A, given in system B</param>
        /// <param name="firstAxis">first axis given</param>
        /// <param name="secondAxis">second axis given</param>
        /// <param name="ignoreScale">if true, axis vectors will be normalized and length will not be taken in count</param>
        /// <returns></returns>
        public static Vector3 ConvertPointToSystemBFromSystemA(Vector3 pointToConvert, Vector3 origin, Vector3 axisOneOfA, Vector3 axisTwoOfA, Axis firstAxis, Axis secondAxis, bool ignoreScale)
        {
            Vector3 xAxisOfA = Vector3.zero;
            Vector3 yAxisOfA = Vector3.zero;
            Vector3 zAxisOfA = Vector3.zero;

            FindPerpToVecs(axisOneOfA, axisTwoOfA, firstAxis, secondAxis, ref xAxisOfA, ref yAxisOfA, ref zAxisOfA);

            Vector3 xAxisNormalized = xAxisOfA.normalized;
            Vector3 yAxisNormalized = yAxisOfA.normalized;
            Vector3 zAxisNormalized = zAxisOfA.normalized;
            Vector3 axisMag = new Vector3(xAxisOfA.magnitude, yAxisOfA.magnitude, zAxisOfA.magnitude);

            Vector3 VecNormalized = ignoreScale ? pointToConvert : UPDBMath.VecTime(pointToConvert, axisMag);

            float x = (VecNormalized.x * xAxisNormalized.x) + (VecNormalized.y * yAxisNormalized.x) + (VecNormalized.z * zAxisNormalized.x);
            float y = (VecNormalized.x * xAxisNormalized.y) + (VecNormalized.y * yAxisNormalized.y) + (VecNormalized.z * zAxisNormalized.y);
            float z = (VecNormalized.x * xAxisNormalized.z) + (VecNormalized.y * yAxisNormalized.z) + (VecNormalized.z * zAxisNormalized.z);

            Vector3 convertedVec = new Vector3(x, y, z);

            return convertedVec + origin;
        }

        #endregion

        #endregion

        #endregion


        #region FindPerpendicularVector

        #region 2D

        /// <summary>
        /// find the vector perpendicular to the given one
        /// </summary>
        /// <param name="givenAxis">vec to dearch for</param>
        /// <param name="axisToFind">wich axis you give ? basically means that : x</param>
        /// <returns></returns>
        public static Vector2 FindPerpToVec(Vector2 givenAxis, Axis axisToFind)
        {
            if (axisToFind != Axis.X && axisToFind != Axis.Y)
            {
                Debug.LogError("ERROR : you're using an axis with a 2D typed vector wich is not X or Y, please select either X or Y axis to give");
                return Vector2.up;
            }

            if (axisToFind == Axis.Y)
                return UPDBMath.FindLeftPerpendicularVector(givenAxis);

            if (axisToFind == Axis.X)
                return UPDBMath.FindRightPerpendicularVector(givenAxis);

            Debug.LogError("ERROR : method " + nameof(FindPerpToVec) + "return no axis possible to give");
            return Vector3.up;
        }

        /// <summary>
        /// find the vector perpendicular to the given one
        /// </summary>
        /// <param name="givenAxis">vec to dearch for</param>
        /// <param name="axisToFind">wich axis you give ? basically means that : x</param>
        /// <returns></returns>
        public static void FindPerpToVec(Vector2 givenAxis, Axis axisToFind, ref Vector2 x, ref Vector2 y)
        {
            if (axisToFind != Axis.X && axisToFind != Axis.Y)
            {
                Debug.LogError("ERROR : you're using an axis with a 2D typed vector wich is not X or Y, please select either X or Y axis to give");
                return;
            }

            if (axisToFind == Axis.Y)
            {
                x = givenAxis;
                y = UPDBMath.FindLeftPerpendicularVector(givenAxis);
                return;
            }

            if (axisToFind == Axis.X)
            {
                x = UPDBMath.FindRightPerpendicularVector(givenAxis);
                y = givenAxis;
                return;
            }

            Debug.LogError("ERROR : method " + nameof(FindPerpToVec) + "return no axis possible to give");
        }

        #endregion

        #region 3D

        /// <summary>
        /// find the vector perpendicular to the given ones
        /// </summary>
        /// <param name="givenAxisA">vec A</param>
        /// <param name="givenAxisB">vec B perpendicular to A</param>
        /// <param name="axisToFind">wich axis you give ? basically means that : x</param>
        /// <returns></returns>
        public static void FindPerpToVecs(Vector3 givenAxisA, Vector3 givenAxisB, Axis firstGivenAxis, Axis secondGivenAxis, ref Vector3 x, ref Vector3 y, ref Vector3 z)
        {
            if (firstGivenAxis == secondGivenAxis || (firstGivenAxis != Axis.X && firstGivenAxis != Axis.Y && firstGivenAxis != Axis.Z) || (secondGivenAxis != Axis.X && secondGivenAxis != Axis.Y && secondGivenAxis != Axis.Z))
            {
                Debug.LogError("ERROR : you setted something wrong with given axis, either first and second are equal, or one of them does not represent either x y or z");
                return;
            }

            if (firstGivenAxis == Axis.X && secondGivenAxis == Axis.Y)
            {
                x = givenAxisA;
                y = givenAxisB;
                z = UPDBMath.FindThirdAxis(x, y);
                return;
            }
            if (firstGivenAxis == Axis.Y && secondGivenAxis == Axis.X)
            {
                x = givenAxisB;
                y = givenAxisA;
                z = UPDBMath.FindThirdAxis(x, y);
                return;
            }

            if (firstGivenAxis == Axis.Y && secondGivenAxis == Axis.Z)
            {
                y = givenAxisA;
                z = givenAxisB;
                x = UPDBMath.FindThirdAxis(y, z);
                return;
            }
            if (firstGivenAxis == Axis.Z && secondGivenAxis == Axis.Y)
            {
                y = givenAxisB;
                z = givenAxisA;
                x = UPDBMath.FindThirdAxis(y, z);
                return;
            }

            if (firstGivenAxis == Axis.X && secondGivenAxis == Axis.Z)
            {
                x = givenAxisA;
                z = givenAxisB;
                y = -UPDBMath.FindThirdAxis(x, z);

                return;
            }
            if (firstGivenAxis == Axis.Z && secondGivenAxis == Axis.X)
            {
                x = givenAxisB;
                z = givenAxisA;
                y = -UPDBMath.FindThirdAxis(x, z);
                return;
            }

            Debug.LogError("ERROR : method " + nameof(FindPerpToVec) + "return no axis possible to give");
        }

        #endregion

        #endregion


        #region Custom Debug

        #region Debug Arrows

        /// <summary>
        /// draw a simple arrow going towards direction
        /// </summary>
        /// <param name="origin">start pos</param>
        /// <param name="direction">direction of arrow</param>
        public static void DebugDrawArrow(Vector3 origin, Vector3 direction)
        {
            float arrowSize = direction.magnitude / 10;
            Vector3 dirBase = direction.normalized * (direction.magnitude - arrowSize);
            Vector3 posBase = origin + dirBase;
            Vector3 perpToBase = UPDBMath.FindAnyPerpendicularVectorUpType(dirBase).normalized * (arrowSize * 0.25f);
            Vector3 perpToPerpToBase = UPDBMath.FindThirdAxis(dirBase, perpToBase, true).normalized * (arrowSize * 0.25f);
            Vector3 crossMinPosA = posBase - perpToBase;
            Vector3 crossMaxPosA = posBase + perpToBase;
            Vector3 crossMinPosB = posBase - perpToPerpToBase;
            Vector3 crossMaxPosB = posBase + perpToPerpToBase;

            //draw base
            Debug.DrawRay(origin, dirBase);

            //draw cross borders
            Debug.DrawLine(crossMinPosA, crossMinPosB);
            Debug.DrawLine(crossMaxPosA, crossMaxPosB);
            Debug.DrawLine(crossMinPosB, crossMaxPosA);
            Debug.DrawLine(crossMaxPosB, crossMinPosA);

            //draw pin
            Debug.DrawLine(crossMinPosA, origin + direction);
            Debug.DrawLine(crossMaxPosA, origin + direction);
            Debug.DrawLine(crossMinPosB, origin + direction);
            Debug.DrawLine(crossMaxPosB, origin + direction);
        }

        /// <summary>
        /// draw a simple arrow going towards direction
        /// </summary>
        /// <param name="origin">start pos</param>
        /// <param name="direction">direction of arrow</param>
        /// <param name="color">color of arrow</param>
        public static void DebugDrawArrow(Vector3 origin, Vector3 direction, Color color)
        {
            float arrowSize = direction.magnitude / 10;
            Vector3 dirBase = direction.normalized * (direction.magnitude - arrowSize);
            Vector3 posBase = origin + dirBase;
            Vector3 perpToBase = UPDBMath.FindAnyPerpendicularVectorUpType(dirBase).normalized * (arrowSize * 0.25f);
            Vector3 perpToPerpToBase = UPDBMath.FindThirdAxis(dirBase, perpToBase, true).normalized * (arrowSize * 0.25f);
            Vector3 crossMinPosA = posBase - perpToBase;
            Vector3 crossMaxPosA = posBase + perpToBase;
            Vector3 crossMinPosB = posBase - perpToPerpToBase;
            Vector3 crossMaxPosB = posBase + perpToPerpToBase;

            //draw base
            Debug.DrawRay(origin, dirBase, color);

            //draw cross borders
            Debug.DrawLine(crossMinPosA, crossMinPosB, color);
            Debug.DrawLine(crossMaxPosA, crossMaxPosB, color);
            Debug.DrawLine(crossMinPosB, crossMaxPosA, color);
            Debug.DrawLine(crossMaxPosB, crossMinPosA, color);

            //draw pin
            Debug.DrawLine(crossMinPosA, origin + direction, color);
            Debug.DrawLine(crossMaxPosA, origin + direction, color);
            Debug.DrawLine(crossMinPosB, origin + direction, color);
            Debug.DrawLine(crossMaxPosB, origin + direction, color);
        }

        /// <summary>
        /// draw an arrow from point start to end
        /// </summary>
        /// <param name="start">starting point</param>
        /// <param name="end">end point</param>
        public static void DebugDrawArrowLine(Vector3 start, Vector3 end)
        {
            Vector3 direction = end - start;
            float arrowSize = direction.magnitude / 10;
            Vector3 dirBase = direction.normalized * (direction.magnitude - arrowSize);
            Vector3 posBase = start + dirBase;
            Vector3 perpToBase = UPDBMath.FindAnyPerpendicularVectorUpType(dirBase).normalized * (arrowSize * 0.25f);
            Vector3 perpToPerpToBase = UPDBMath.FindThirdAxis(dirBase, perpToBase, true).normalized * (arrowSize * 0.25f);
            Vector3 crossMinPosA = posBase - perpToBase;
            Vector3 crossMaxPosA = posBase + perpToBase;
            Vector3 crossMinPosB = posBase - perpToPerpToBase;
            Vector3 crossMaxPosB = posBase + perpToPerpToBase;

            //draw base
            Debug.DrawRay(start, dirBase);

            //draw cross borders
            Debug.DrawLine(crossMinPosA, crossMinPosB);
            Debug.DrawLine(crossMaxPosA, crossMaxPosB);
            Debug.DrawLine(crossMinPosB, crossMaxPosA);
            Debug.DrawLine(crossMaxPosB, crossMinPosA);

            //draw pin
            Debug.DrawLine(crossMinPosA, end);
            Debug.DrawLine(crossMaxPosA, end);
            Debug.DrawLine(crossMinPosB, end);
            Debug.DrawLine(crossMaxPosB, end);
        }

        /// <summary>
        /// draw an arrow from point start to end
        /// </summary>
        /// <param name="start">starting point</param>
        /// <param name="end">end point</param>
        /// <param name="color">color of arrow</param>
        public static void DebugDrawArrowLine(Vector3 start, Vector3 end, Color color)
        {
            Vector3 direction = end - start;
            float arrowSize = direction.magnitude / 10;
            Vector3 dirBase = direction.normalized * (direction.magnitude - arrowSize);
            Vector3 posBase = start + dirBase;
            Vector3 perpToBase = UPDBMath.FindAnyPerpendicularVectorUpType(dirBase).normalized * (arrowSize * 0.25f);
            Vector3 perpToPerpToBase = UPDBMath.FindThirdAxis(dirBase, perpToBase, true).normalized * (arrowSize * 0.25f);
            Vector3 crossMinPosA = posBase - perpToBase;
            Vector3 crossMaxPosA = posBase + perpToBase;
            Vector3 crossMinPosB = posBase - perpToPerpToBase;
            Vector3 crossMaxPosB = posBase + perpToPerpToBase;

            //draw base
            Debug.DrawRay(start, dirBase, color);

            //draw cross borders
            Debug.DrawLine(crossMinPosA, crossMinPosB, color);
            Debug.DrawLine(crossMaxPosA, crossMaxPosB, color);
            Debug.DrawLine(crossMinPosB, crossMaxPosA, color);
            Debug.DrawLine(crossMaxPosB, crossMinPosA, color);

            //draw pin
            Debug.DrawLine(crossMinPosA, end, color);
            Debug.DrawLine(crossMaxPosA, end, color);
            Debug.DrawLine(crossMinPosB, end, color);
            Debug.DrawLine(crossMaxPosB, end, color);
        }

        /// <summary>
        /// draw a simple arrow going towards direction, in 2D
        /// </summary>
        /// <param name="start">start pos</param>
        /// <param name="direction">direction of arrow</param>
        public static void DebugDrawArrow(Vector2 start, Vector2 direction)
        {
            float arrowSize = direction.magnitude / 10;
            Vector2 dirBase = direction.normalized * (direction.magnitude - arrowSize);
            Vector2 posBase = start + dirBase;
            Vector2 perpToBase = UPDBMath.FindRightPerpendicularVector(direction).normalized * (arrowSize * 0.25f);
            Vector2 crossMinPos = posBase - perpToBase;
            Vector2 crossMaxPos = posBase + perpToBase;

            //draw base
            Debug.DrawRay(start, dirBase);

            //draw cross borders
            Debug.DrawLine(crossMinPos, crossMaxPos);

            //draw pin
            Debug.DrawLine(crossMinPos, start + direction);
            Debug.DrawLine(crossMaxPos, start + direction);

        }

        /// <summary>
        /// draw a simple arrow going towards direction, in 2D
        /// </summary>
        /// <param name="origin">start pos</param>
        /// <param name="direction">direction of arrow</param>
        /// <param name="color">color of arrow</param>
        public static void DebugDrawArrow(Vector2 origin, Vector2 direction, Color color)
        {
            float arrowSize = direction.magnitude / 10;
            Vector2 dirBase = direction.normalized * (direction.magnitude - arrowSize);
            Vector2 posBase = origin + dirBase;
            Vector2 perpToBase = UPDBMath.FindRightPerpendicularVector(direction).normalized * (arrowSize * 0.25f);
            Vector2 crossMinPos = posBase - perpToBase;
            Vector2 crossMaxPos = posBase + perpToBase;

            //draw base
            Debug.DrawRay(origin, dirBase, color);

            //draw cross borders
            Debug.DrawLine(crossMinPos, crossMaxPos, color);

            //draw pin
            Debug.DrawLine(crossMinPos, origin + direction, color);
            Debug.DrawLine(crossMaxPos, origin + direction, color);

        }

        /// <summary>
        /// draw an arrow from point start to end, in 2D
        /// </summary>
        /// <param name="start">start pos</param>
        /// <param name="end">end pos</param>
        public static void DebugDrawArrowLine(Vector2 start, Vector2 end)
        {
            Vector2 direction = end - start;
            float arrowSize = direction.magnitude / 10;
            Vector2 dirBase = direction.normalized * (direction.magnitude - arrowSize);
            Vector2 posBase = start + dirBase;
            Vector2 perpToBase = UPDBMath.FindRightPerpendicularVector(direction).normalized * (arrowSize * 0.25f);
            Vector2 crossMinPos = posBase - perpToBase;
            Vector2 crossMaxPos = posBase + perpToBase;

            //draw base
            Debug.DrawRay(start, dirBase);

            //draw cross borders
            Debug.DrawLine(crossMinPos, crossMaxPos);

            //draw pin
            Debug.DrawLine(crossMinPos, end);
            Debug.DrawLine(crossMaxPos, end);

        }

        /// <summary>
        /// draw an arrow from point start to end, in 2D
        /// </summary>
        /// <param name="start">start pos</param>
        /// <param name="end">end pos</param>
        /// <param name="color">color of arrow</param>
        public static void DebugDrawArrowLine(Vector2 start, Vector2 end, Color color)
        {
            Vector2 direction = end - start;
            float arrowSize = direction.magnitude / 10;
            Vector2 dirBase = direction.normalized * (direction.magnitude - arrowSize);
            Vector2 posBase = start + dirBase;
            Vector2 perpToBase = UPDBMath.FindRightPerpendicularVector(direction).normalized * (arrowSize * 0.25f);
            Vector2 crossMinPos = posBase - perpToBase;
            Vector2 crossMaxPos = posBase + perpToBase;

            //draw base
            Debug.DrawRay(start, dirBase, color);

            //draw cross borders
            Debug.DrawLine(crossMinPos, crossMaxPos, color);

            //draw pin
            Debug.DrawLine(crossMinPos, end, color);
            Debug.DrawLine(crossMaxPos, end, color);

        }

        #endregion

        #region Debug Points

        /// <summary>
        /// draw a point
        /// </summary>
        /// <param name="position">position of point</param>
        public static void DebugDrawPoint(Vector3 position)
        {
            float size = .1f;
            Vector3 up = Vector3.up * size;
            Vector3 right = Vector3.right * size;
            Vector3 forward = Vector3.forward * size;

            Debug.DrawLine(position - right, position + right);
            Debug.DrawLine(position - up, position + up);
            Debug.DrawLine(position - forward, position + forward);
        }

        /// <summary>
        /// draw a point
        /// </summary>
        /// <param name="position">position of point</param>
        /// <param name="color">color of point</param>
        public static void DebugDrawPoint(Vector3 position, Color color)
        {
            float size = .1f;
            Vector3 up = Vector3.up * size;
            Vector3 right = Vector3.right * size;
            Vector3 forward = Vector3.forward * size;

            Debug.DrawLine(position - right, position + right, color);
            Debug.DrawLine(position - up, position + up, color);
            Debug.DrawLine(position - forward, position + forward, color);
        }

        /// <summary>
        /// draw a point
        /// </summary>
        /// <param name="position">position of point</param>
        /// <param name="adaptToSceneView">adapt point scale with scene view distance</param>
        public static void DebugDrawPoint(Vector3 position, bool adaptToSceneView)
        {
            float size = adaptToSceneView && Camera.current ? Vector3.Distance(position, Camera.current.transform.position) * .025f : .1f;
            Vector3 up = Vector3.up * size;
            Vector3 right = Vector3.right * size;
            Vector3 forward = Vector3.forward * size;

            Debug.DrawLine(position - right, position + right);
            Debug.DrawLine(position - up, position + up);
            Debug.DrawLine(position - forward, position + forward);
        }

        /// <summary>
        /// draw a point
        /// </summary>
        /// <param name="position">position of point</param>
        /// <param name="color">color of point</param>
        /// <param name="adaptToSceneView">adapt point scale with scene view distance</param>
        public static void DebugDrawPoint(Vector3 position, Color color, bool adaptToSceneView)
        {
            float size = adaptToSceneView && Camera.current ? Vector3.Distance(position, Camera.current.transform.position) * .025f : .1f;
            Vector3 up = Vector3.up * size;
            Vector3 right = Vector3.right * size;
            Vector3 forward = Vector3.forward * size;

            Debug.DrawLine(position - right, position + right, color);
            Debug.DrawLine(position - up, position + up, color);
            Debug.DrawLine(position - forward, position + forward, color);
        }

        /// <summary>
        /// draw a point
        /// </summary>
        /// <param name="position">position of point</param>
        public static void DebugDrawPoint(Vector2 position)
        {
            float size = .1f;
            Vector2 up = Vector2.up * size;
            Vector2 right = Vector2.right * size;

            Debug.DrawLine(position - right, position + right);
            Debug.DrawLine(position - up, position + up);
        }

        /// <summary>
        /// draw a point
        /// </summary>
        /// <param name="position">position of point</param>
        /// <param name="color">color of point</param>
        public static void DebugDrawPoint(Vector2 position, Color color)
        {
            float size = .1f;
            Vector2 up = Vector2.up * size;
            Vector2 right = Vector2.right * size;

            Debug.DrawLine(position - right, position + right, color);
            Debug.DrawLine(position - up, position + up, color);
        }

        /// <summary>
        /// draw a point
        /// </summary>
        /// <param name="position">position of point</param>
        /// <param name="adaptToSceneView">adapt point scale with scene view distance</param>
        public static void DebugDrawPoint(Vector2 position, bool adaptToSceneView)
        {
            float size = adaptToSceneView && Camera.current ? Vector3.Distance(position, new Vector3(position.x, position.y, Camera.current.transform.position.z)) * .025f : .1f;
            Vector2 up = Vector2.up * size;
            Vector2 right = Vector2.right * size;

            Debug.DrawLine(position - right, position + right);
            Debug.DrawLine(position - up, position + up);
        }

        /// <summary>
        /// draw a point
        /// </summary>
        /// <param name="position">position of point</param>
        /// <param name="color">color of point</param>
        /// <param name="adaptToSceneView">adapt point scale with scene view distance</param>
        public static void DebugDrawPoint(Vector2 position, Color color, bool adaptToSceneView)
        {
            float size = adaptToSceneView && Camera.current ? Vector3.Distance(position, new Vector3(position.x, position.y, Camera.current.transform.position.z)) * .025f : .1f;
            Vector2 up = Vector2.up * size;
            Vector2 right = Vector2.right * size;

            Debug.DrawLine(position - right, position + right, color);
            Debug.DrawLine(position - up, position + up, color);
        }

        #endregion

        #region DrawGrid

        #region 2DOverrides

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCount">how many edges are rendered in three axis ?</param>
        /// <param name="gridColor">color of grid</param>
        public static void DebugDrawGrid(Vector2 origin, int edgeCount, Color gridColor)
        {
            Vector3 right = Vector3.right;
            Vector3 up = Vector3.up;
            Vector3 forward = Vector3.forward;

            if (edgeCount != 0)
                DrawLineSection(origin, edgeCount, edgeCount, 0, right, up, forward, gridColor, PositionRendredType.Centered);

            if (edgeCount != 0)
                DrawLineSection(origin, edgeCount, edgeCount, 0, up, right, forward, gridColor, PositionRendredType.Centered);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCount">how many edges are rendered in three axis ?</param>
        /// <param name="localFrom">what is the object representing right, up, and forward axis ?</param>
        /// <param name="gridColor">color of grid</param>
        public static void DebugDrawGrid(Vector2 origin, int edgeCount, Transform localFrom, Color gridColor)
        {
            Vector3 right = localFrom.right;
            Vector3 up = localFrom.up;
            Vector3 forward = Vector3.forward;

            if (edgeCount != 0)
                DrawLineSection(origin, edgeCount, edgeCount, 0, right, up, forward, gridColor, PositionRendredType.Centered);

            if (edgeCount != 0)
                DrawLineSection(origin, edgeCount, edgeCount, 0, up, right, forward, gridColor, PositionRendredType.Centered);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="gridColor">color of grid</param>
        public static void DebugDrawGrid(Vector2 origin, int edgeCountX, int edgeCountY, Color gridColor)
        {
            Vector3 right = Vector3.right;
            Vector3 up = Vector3.up;
            Vector3 forward = Vector3.forward;

            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, 0, right, up, forward, gridColor, PositionRendredType.Centered);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, 0, up, right, forward, gridColor, PositionRendredType.Centered);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="gridColor">color of grid</param>
        /// <param name="positionType">position type of grid</param>
        public static void DebugDrawGrid(Vector2 origin, int edgeCountX, int edgeCountY, Color gridColor, PositionRendredType positionType)
        {
            Vector3 right = Vector3.right;
            Vector3 up = Vector3.up;
            Vector3 forward = Vector3.forward;

            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, 0, right, up, forward, gridColor, positionType);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, 0, up, right, forward, gridColor, positionType);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="scale">what are the scale of the three axis ?</param>
        /// <param name="gridColor">color of grid</param>
        public static void DebugDrawGrid(Vector2 origin, int edgeCountX, int edgeCountY, Vector3 scale, Color gridColor)
        {
            Vector3 right = Vector3.right * scale.x;
            Vector3 up = Vector3.up * scale.y;
            Vector3 forward = Vector3.forward * scale.z;

            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, 0, right, up, forward, gridColor, PositionRendredType.Centered);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, 0, up, right, forward, gridColor, PositionRendredType.Centered);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="scale">what are the scale of the two axis ?</param>
        /// <param name="gridColor">color of grid</param>
        /// <param name="positionType">position type of grid</param>
        public static void DebugDrawGrid(Vector2 origin, int edgeCountX, int edgeCountY, Vector2 scale, Color gridColor, PositionRendredType positionType)
        {
            Vector3 right = Vector3.right * scale.x;
            Vector3 up = Vector3.up * scale.y;
            Vector3 forward = Vector3.forward;

            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, 0, right, up, forward, gridColor, positionType);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, 0, up, right, forward, gridColor, positionType);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="right">represent x axis of grid</param>
        /// <param name="up">represent y axis of grid</param>
        /// <param name="gridColor">color of grid</param>
        public static void DebugDrawGrid(Vector2 origin, int edgeCountX, int edgeCountY, Vector3 right, Vector3 up, Color gridColor)
        {
            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, 0, right, up, Vector3.forward, gridColor, PositionRendredType.Centered);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, 0, up, right, Vector3.forward, gridColor, PositionRendredType.Centered);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="right">represent x axis of grid</param>
        /// <param name="up">represent y axis of grid</param>
        /// <param name="gridColor">color of grid</param>
        /// <param name="positionType">position type of grid</param>
        public static void DebugDrawGrid(Vector2 origin, int edgeCountX, int edgeCountY, Vector3 right, Vector3 up, Color gridColor, PositionRendredType positionType)
        {
            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, 0, right, up, Vector3.forward, gridColor, positionType);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, 0, up, right, Vector3.forward, gridColor, positionType);
        }

        #endregion

        #region DrawGrid3D

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCount">how many edges are rendered in three axis ?</param>
        /// <param name="gridColor">color of grid</param>
        public static void DebugDrawGrid(Vector3 origin, int edgeCount, Color gridColor)
        {
            Vector3 right = Vector3.right;
            Vector3 up = Vector3.up;
            Vector3 forward = Vector3.forward;

            if (edgeCount != 0)
                DrawLineSection(origin, edgeCount, edgeCount, edgeCount, right, up, forward, gridColor, PositionRendredType.Centered);

            if (edgeCount != 0)
                DrawLineSection(origin, edgeCount, edgeCount, edgeCount, up, right, forward, gridColor, PositionRendredType.Centered);

            if (edgeCount != 0)
                DrawLineSection(origin, edgeCount, edgeCount, edgeCount, forward, up, right, gridColor, PositionRendredType.Centered);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCount">how many edges are rendered in three axis ?</param>
        /// <param name="localFrom">what is the object representing right, up, and forward axis ?</param>
        /// <param name="gridColor">color of grid</param>
        public static void DebugDrawGrid(Vector3 origin, int edgeCount, Transform localFrom, Color gridColor)
        {
            Vector3 right = localFrom.right;
            Vector3 up = localFrom.up;
            Vector3 forward = localFrom.forward;

            if (edgeCount != 0)
                DrawLineSection(origin, edgeCount, edgeCount, edgeCount, right, up, forward, gridColor, PositionRendredType.Centered);

            if (edgeCount != 0)
                DrawLineSection(origin, edgeCount, edgeCount, edgeCount, up, right, forward, gridColor, PositionRendredType.Centered);

            if (edgeCount != 0)
                DrawLineSection(origin, edgeCount, edgeCount, edgeCount, forward, up, right, gridColor, PositionRendredType.Centered);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="edgeCountZ">how many edges are rendered in z axis ?</param>
        /// <param name="gridColor">color of grid</param>
        public static void DebugDrawGrid(Vector3 origin, int edgeCountX, int edgeCountY, int edgeCountZ, Color gridColor)
        {
            Vector3 right = Vector3.right;
            Vector3 up = Vector3.up;
            Vector3 forward = Vector3.forward;

            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, edgeCountZ, right, up, forward, gridColor, PositionRendredType.Centered);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, edgeCountZ, up, right, forward, gridColor, PositionRendredType.Centered);

            if (edgeCountZ != 0)
                DrawLineSection(origin, edgeCountZ, edgeCountY, edgeCountX, forward, up, right, gridColor, PositionRendredType.Centered);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="edgeCountZ">how many edges are rendered in z axis ?</param>
        /// <param name="gridColor">color of grid</param>
        /// <param name="positionType">position type of grid</param>
        public static void DebugDrawGrid(Vector3 origin, int edgeCountX, int edgeCountY, int edgeCountZ, Color gridColor, PositionRendredType positionType)
        {
            Vector3 right = Vector3.right;
            Vector3 up = Vector3.up;
            Vector3 forward = Vector3.forward;

            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, edgeCountZ, right, up, forward, gridColor, positionType);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, edgeCountZ, up, right, forward, gridColor, positionType);

            if (edgeCountZ != 0)
                DrawLineSection(origin, edgeCountZ, edgeCountY, edgeCountX, forward, up, right, gridColor, positionType);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="edgeCountZ">how many edges are rendered in z axis ?</param>
        /// <param name="scale">what are the scale of the three axis ?</param>
        /// <param name="gridColor">color of grid</param>
        public static void DebugDrawGrid(Vector3 origin, int edgeCountX, int edgeCountY, int edgeCountZ, Vector3 scale, Color gridColor)
        {
            Vector3 right = Vector3.right * scale.x;
            Vector3 up = Vector3.up * scale.y;
            Vector3 forward = Vector3.forward * scale.z;

            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, edgeCountZ, right, up, forward, gridColor, PositionRendredType.Centered);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, edgeCountZ, up, right, forward, gridColor, PositionRendredType.Centered);

            if (edgeCountZ != 0)
                DrawLineSection(origin, edgeCountZ, edgeCountY, edgeCountX, forward, up, right, gridColor, PositionRendredType.Centered);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="edgeCountZ">how many edges are rendered in z axis ?</param>
        /// <param name="scale">what are the scale of the three axis ?</param>
        /// <param name="gridColor">color of grid</param>
        /// <param name="positionType">position type of grid</param>
        public static void DebugDrawGrid(Vector3 origin, int edgeCountX, int edgeCountY, int edgeCountZ, Vector3 scale, Color gridColor, PositionRendredType positionType)
        {
            Vector3 right = Vector3.right * scale.x;
            Vector3 up = Vector3.up * scale.y;
            Vector3 forward = Vector3.forward * scale.z;

            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, edgeCountZ, right, up, forward, gridColor, positionType);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, edgeCountZ, up, right, forward, gridColor, positionType);

            if (edgeCountZ != 0)
                DrawLineSection(origin, edgeCountZ, edgeCountY, edgeCountX, forward, up, right, gridColor, positionType);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="edgeCountZ">how many edges are rendered in z axis ?</param>
        /// <param name="right">represent x axis of grid</param>
        /// <param name="up">represent y axis of grid</param>
        /// <param name="forward">represent z axis of grid</param>
        /// <param name="gridColor">color of grid</param>
        public static void DebugDrawGrid(Vector3 origin, int edgeCountX, int edgeCountY, int edgeCountZ, Vector3 right, Vector3 up, Vector3 forward, Color gridColor)
        {
            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, edgeCountZ, right, up, forward, gridColor, PositionRendredType.Centered);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, edgeCountZ, up, right, forward, gridColor, PositionRendredType.Centered);

            if (edgeCountZ != 0)
                DrawLineSection(origin, edgeCountZ, edgeCountY, edgeCountX, forward, up, right, gridColor, PositionRendredType.Centered);
        }

        /// <summary>
        /// draw a grid, adapted for coordinate renderer
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountX">how many edges are rendered in x axis ?</param>
        /// <param name="edgeCountY">how many edges are rendered in y axis ?</param>
        /// <param name="edgeCountZ">how many edges are rendered in z axis ?</param>
        /// <param name="right">represent x axis of grid</param>
        /// <param name="up">represent y axis of grid</param>
        /// <param name="forward">represent z axis of grid</param>
        /// <param name="gridColor">color of grid</param>
        /// <param name="positionType">position type of grid</param>
        public static void DebugDrawGrid(Vector3 origin, int edgeCountX, int edgeCountY, int edgeCountZ, Vector3 right, Vector3 up, Vector3 forward, Color gridColor, PositionRendredType positionType)
        {
            if (edgeCountX != 0)
                DrawLineSection(origin, edgeCountX, edgeCountY, edgeCountZ, right, up, forward, gridColor, positionType);

            if (edgeCountY != 0)
                DrawLineSection(origin, edgeCountY, edgeCountX, edgeCountZ, up, right, forward, gridColor, positionType);

            if (edgeCountZ != 0)
                DrawLineSection(origin, edgeCountZ, edgeCountY, edgeCountX, forward, up, right, gridColor, positionType);
        }

        #endregion

        /// <summary>
        /// take given parameters, and draw a section of lines in the axisToDraw direction
        /// </summary>
        /// <param name="origin">what is the central point of grid</param>
        /// <param name="edgeCountToDraw">how many edges are rendered in to draw axis ?</param>
        /// <param name="edgeCountOne">how many edges are rendered in first axis ?</param>
        /// <param name="edgeCountTwo">how many edges are rendered in second axis ?</param>
        /// <param name="axisToDraw">axis to draw towards</param>
        /// <param name="axisOne">first axis reprensenting second dimention axis</param>
        /// <param name="axisTwo">second axis reprensenting third dimention axis</param>
        /// <param name="gridColor"></param>
        /// <param name="positionType"></param>
        public static void DrawLineSection(Vector3 origin, int edgeCountToDraw, int edgeCountOne, int edgeCountTwo, Vector3 axisToDraw, Vector3 axisOne, Vector3 axisTwo, Color gridColor, PositionRendredType positionType)
        {
            Vector3 offset = TestGridOffsetType(edgeCountToDraw, edgeCountOne, edgeCountTwo, axisToDraw, axisOne, axisTwo, positionType);

            for (int i = 0; i < edgeCountOne + 1; i++)
            {
                for (int j = 0; j < edgeCountTwo + 1; j++)
                {
                    Vector3 axisToDrawLength = axisToDraw * edgeCountToDraw;
                    Vector3 lowPoint = origin;
                    Vector3 highPoint = origin + axisToDrawLength;
                    Vector3 firstAxisILength = axisOne * i;
                    Vector3 secondAxisILength = axisTwo * j;
                    Vector3 firstAndSecondAxisOffset = firstAxisILength + secondAxisILength;
                    Vector3 A = lowPoint + firstAndSecondAxisOffset;
                    Vector3 B = highPoint + firstAndSecondAxisOffset;

                    Debug.DrawLine(A - offset, B - offset, gridColor);
                }
            }
        }

        /// <summary>
        /// take the numbers of count lines to draw, and the third axis, and return the offset depending on the position type
        /// </summary>
        /// <param name="edgeCountToDraw">how many edges are rendered in to draw axis ?</param>
        /// <param name="edgeCountOne">how many edges are rendered in first axis ?</param>
        /// <param name="edgeCountTwo">how many edges are rendered in second axis ?</param>
        /// <param name="axisToDraw">axis to draw towards</param>
        /// <param name="axisOne">first axis reprensenting second dimention axis</param>
        /// <param name="axisTwo">second axis reprensenting third dimention axis</param>
        /// <param name="positionType"></param>
        /// <returns></returns>
        private static Vector3 TestGridOffsetType(int edgeCountToDraw, int edgeCountOne, int edgeCountTwo, Vector3 axisToDraw, Vector3 axisOne, Vector3 axisTwo, PositionRendredType positionType)
        {
            if (positionType == PositionRendredType.IntCentered)
            {
                Vector3 toDrawOffset = axisToDraw * (edgeCountToDraw / 2);
                Vector3 firstOffset = axisOne * (edgeCountOne / 2);
                Vector3 secondOffset = axisTwo * (edgeCountTwo / 2);

                return toDrawOffset + firstOffset + secondOffset;
            }
            if (positionType == PositionRendredType.Edged)
            {
                return Vector3.zero;
            }
            if (positionType == PositionRendredType.Centered)
            {
                Vector3 toDrawOffset = axisToDraw * (edgeCountToDraw / 2f);
                Vector3 firstOffset = axisOne * (edgeCountOne / 2f);
                Vector3 secondOffset = axisTwo * (edgeCountTwo / 2f);

                return toDrawOffset + firstOffset + secondOffset;
            }

            Debug.LogError("EXCEPTION : grid offset type takes a position type that doesn't exist !");
            return Vector3.zero;
        }

        #endregion

        #endregion
    }
}
