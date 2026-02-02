using System.Collections.Generic;
using UnityEngine;
using ScriptableObjects;

public class CommonFunctions
{

    public static bool IsColliderWithTag(Collider collider, string tagToCompare)
    {
        return collider.gameObject.tag == tagToCompare;
    }
    
    public static bool IsColliderWithOneOfTags(Collider collider, List<string> tagsToCompare)
    {
        return tagsToCompare.Contains(collider.gameObject.tag);
    }

    public static ContactPoint GetCollisionContactPoint(Collision collision)
    {
        return collision.contacts[0];
    }

    public static Vector3 GetCollisionPosition(Collision collision)
    {
        return collision.contacts[0].point;
    }
    
    public static Quaternion GetCollisionRotation(Collision collision)
    {
        return Quaternion.FromToRotation(Vector3.up, collision.contacts[0].normal);
    }

    public static void PlayOrStopAllParticleSystems(List<ParticleSystem> psArr, bool isStop = false)
    {
        foreach (ParticleSystem ps in psArr)
        {
            if (isStop)
            {
                ps.Stop();
            } else
            {
                ps.Play();
            }
        }
    }
    public static float GetMaterialEmission(Material mat)
    {
        return mat.GetColor("_EmissionColor").a;
    }
    public static void SetMaterialEmissionIntensity(Material mat, float intensity)
    {
        mat.SetColor("_EmissionColor", mat.color * intensity);
    }
    public static bool IsAnimationPlaying(Animator anim, string stateName)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsAnimationPassedPrecentage(Animator anim, string stateName, float precentage)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime > precentage)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsCollisionWithTag(Collision collision, string tagToCompare)
    {
        return collision.gameObject.tag == tagToCompare;
    }

    public static bool IsRaycastHitWithTag(RaycastHit hit, string tagToCompare)
    {
        if (!hit.transform) return false;
        //Debug.Log($"Hit with: {hit.transform.tag}");

        return hit.transform.tag == tagToCompare;
    }
    
    public static bool IsRaycastHitWithAnyOfThoseTags(RaycastHit hit, List<string> tags)
    {
        if (!hit.transform) return false;
        return tags.Contains(hit.transform.tag);
    }

    public static void PrintList<T>(List<T> list, string prefix = "")
    {
        string result = prefix + " " + "List contents: ";
        foreach (var item in list)
        {
            result += item.ToString() + ", ";
        }
        Debug.Log(result);
    }

    public static string PrintListInTemplate<T>(List<T> list, string prefix = "")
    {
        string result = prefix;
        foreach (var item in list)
        {
            result += item.ToString() + ", ";
        }
        return result;
    }

    public static bool IsThereObjectsAroundWithTag(Vector3 position, float radius, string tagToCompare)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, radius);
        foreach (var hitCollider in hitColliders)
        {
            return IsColliderWithTag(hitCollider, tagToCompare);
        }
        return false;
    }
    
    public static Collider GetColliderAroundWithTag(Vector3 position, float radius, string tagToCompare)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, radius);

        foreach (var hitCollider in hitColliders)
        {
            if (IsColliderWithTag(hitCollider, tagToCompare))
            {
                return hitCollider;
            }
        }
        return null;
    }

    public static List<Collider> GetAllObjectsAroundWithTag(Vector3 position, float radius, string tagToCompare)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, radius);
        List<Collider> hitCollidersWithTag = new List<Collider>();
        foreach (var hitCollider in hitColliders)
        {
            if (IsColliderWithTag(hitCollider, tagToCompare))
            {
                hitCollidersWithTag.Add(hitCollider);
            }
        }
        return hitCollidersWithTag;
    }

    public static List<Collider> GetAllObjectsAroundWithTag(Vector3 position, float radius, List<string> tagsToCompare)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, radius);
        List<Collider> hitCollidersWithTag = new List<Collider>();
        foreach (var hitCollider in hitColliders)
        {
            if (IsColliderWithOneOfTags(hitCollider, tagsToCompare))
            {
                hitCollidersWithTag.Add(hitCollider);
            }
        }
        return hitCollidersWithTag;
    }
    public static List<Collider> GetAllObjectsAroundWithTag(Vector3 position, float radius, List<string> tagsToCompare, LayerMask layer)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, radius, layer);
        List<Collider> hitCollidersWithTag = new List<Collider>();
        foreach (var hitCollider in hitColliders)
        {
            if (IsColliderWithOneOfTags(hitCollider, tagsToCompare))
            {
                hitCollidersWithTag.Add(hitCollider);
            }
        }
        return hitCollidersWithTag;
    }

    public static RaycastHit GetRayHit(Vector3 rayOrigin, Vector3 rayDirection, float distance)
    {
        RaycastHit hit;
        Physics.Raycast(rayOrigin, rayDirection, out hit, distance);
        Debug.DrawRay(rayOrigin, rayDirection * distance, Color.red);
        return hit;
    }
    
    public static RaycastHit GetRayHit(Vector3 rayOrigin, Vector3 rayDirection, float distance, LayerMask layer)
    {
        RaycastHit hit;
        Physics.Raycast(rayOrigin, rayDirection, out hit, distance, layer);
        Debug.DrawRay(rayOrigin, rayDirection * distance, Color.red);
        return hit;
    }

    public static RaycastHit GetRayHit(Vector3 rayOrigin, Vector3 rayDirection, float distance, Color rayColor)
    {
        RaycastHit hit;
        Physics.Raycast(rayOrigin, rayDirection, out hit, distance);
        Debug.DrawRay(rayOrigin, rayDirection * distance, rayColor);
        return hit;
    }
    public static RaycastHit GetRayHit(Vector3 rayOrigin, Vector3 rayDirection, float distance, Color rayColor, LayerMask layer)
    {
        RaycastHit hit;
        Physics.Raycast(rayOrigin, rayDirection, out hit, distance, layer);
        Debug.DrawRay(rayOrigin, rayDirection * distance, rayColor);
        return hit;
    }

    public static Vector2 get2DPosition(Vector3 Position)
    {
        Vector2 Pos = new Vector2(Position.x, Position.y);
        return Pos;
    }

    public static Material GetMaterialInstanceByName(MeshRenderer mr, Material originalMat)
    {
        foreach (Material mat in mr.materials)
        {
            // When creating a material instance it's adding (Instance) to the material name, so i had to remove it in order to find the material
            // i was looking for
            if (mat.name.Replace(" (Instance)", "") == originalMat.name)
            {
                return mat;
            }

        }
        return null;
    }
    
    public static void ChangeMaterialInMeshRenderer(ref MeshRenderer mr, Material matToSwich, Material newMat)
    {
        for (int i = 0; i < mr.materials.Length; i++)
        {
            Material mat = mr.materials[i];
            if (mat.name.Replace(" (Instance)", "") == matToSwich.name)
            {
                Material[] newMatList = mr.materials;
                newMatList[i] = newMat;
                mr.materials = newMatList;
            }
        }
    }
    public static void ChangeMaterialInMeshRenderer(ref SkinnedMeshRenderer smr, Material matToSwich, Material newMat)
    {
        for (int i = 0; i < smr.materials.Length; i++)
        {
            Material mat = smr.materials[i];
            if (mat.name.Replace(" (Instance)", "") == matToSwich.name)
            {
                Material[] newMatList = smr.materials;
                newMatList[i] = newMat;
                smr.materials = newMatList;
            }
        }
    }

    public static Material GetMaterialInstanceByName(List<Material> materials, Material originalMat)
    {
        foreach (Material mat in materials)
        {
            // When creating a material instance it's adding (Instance) to the material name, so i had to remove it in order to find the material
            // i was looking for
            if (mat.name.Replace(" (Instance)", "") == originalMat.name)
            {
                return mat;
            }

        }
        return null;
    }

    public static void SetParticleRateOverTimeEmission(ParticleSystem effect, float value)
    {
        var emission = effect.emission;
        emission.rateOverTime = value;
    }
    public static void SetParticleRateOverTimeEmissionAndPlay(ParticleSystem effect, float value)
    {
        var emission = effect.emission;
        emission.rateOverTime = value;
        effect.Play();
    }

    public static float GetCurrentAnimationDuration(Animator anim)
    {
        return anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;
    }
    public static float GetAnimationDuration(Animator anim, string animName)
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == animName) return clip.length;
        }
        return 0f;
    }

    public static float JumpVelocityToHight(float height, float gravity)
    {
        return Mathf.Sqrt(height * 2 * gravity);
    }

    public static bool IsClose(Vector3 target, Vector3 detector, float distance, float heightOffset = 0.0f)
    {
        Vector3 eyePos = detector + Vector3.up * heightOffset;
        Vector3 toPlayer = target - eyePos;
        return toPlayer.sqrMagnitude <= distance * distance;
    }

    public static bool InView(Transform target, Transform detector, float angle, float radius, float heightOffset, float maxHeightDifference, LayerMask viewBlockerLayerMask, bool useHeightDifference = true)
    {
        return InView(target.position, detector.position, detector.forward, angle, radius, heightOffset, maxHeightDifference, viewBlockerLayerMask, useHeightDifference);
    }
    public static bool InView(Vector3 target, Vector3 detector, Vector3 detectorDirection, float angle, float radius, float heightOffset, float maxHeightDifference, LayerMask viewBlockerLayerMask, bool useHeightDifference = true)
    {
        Vector3 eyePos = detector + Vector3.up * heightOffset;
        Vector3 toPlayer = target - eyePos;
        Vector3 toPlayerTop = target + Vector3.up * 1.5f - eyePos;

        if (useHeightDifference && Mathf.Abs(toPlayer.y + heightOffset) > maxHeightDifference)
        { //if the target is too high or too low no need to try to reach it, just abandon pursuit
            return false;
        }

        Vector3 toPlayerFlat = toPlayer;
        toPlayerFlat.y = 0;

        if (toPlayerFlat.sqrMagnitude <= radius * radius)
        {
            if (Vector3.Dot(toPlayerFlat.normalized, detectorDirection) >
                Mathf.Cos(angle * 0.5f * Mathf.Deg2Rad))
            {

                bool canSee = false;

                Debug.DrawRay(eyePos, toPlayer, Color.blue);
                Debug.DrawRay(eyePos, toPlayerTop, Color.blue);

                canSee |= !Physics.Raycast(eyePos, toPlayer.normalized, radius,
                    viewBlockerLayerMask, QueryTriggerInteraction.Ignore);

                canSee |= !Physics.Raycast(eyePos, toPlayerTop.normalized, toPlayerTop.magnitude,
                    viewBlockerLayerMask, QueryTriggerInteraction.Ignore);

                if (canSee)
                    return true;
            }
        }

        return false;
    }
    public static float AngleToDot(float angle)
    {
        Vector3 one = Vector3.forward;
        Vector3 two = Matrix4x4.Rotate(Quaternion.Euler(0.0f, angle, 0.0f)) * Vector3.forward;
        return Vector3.Dot(one, two);

        //return Mathf.Cos(angle);
    }

    public static bool IsTagInList(string tag, string[] array)
    {
        foreach(var a in array)
        {
            if (tag == a)
            {
                return true;
            }
        }
        return false;
    }
    public static bool IsTagInList(string tag, List<string> list)
    {
        return IsTagInList(tag, list.ToArray());
    }

    public static Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // Quadratic Bezier formula: (1 - t)^2 * p0 + 2 * (1 - t) * t * p1 + t^2 * p2
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 point = uu * p0; // (1 - t)^2 * p0
        point += 2 * u * t * p1; // 2 * (1 - t) * t * p1
        point += tt * p2; // t^2 * p2
        return point;
    }

    public static Vector3 Anticipate(Vector3 source, Vector3 target, Vector3 tDirection, float sSpeed, float tSpeed)
    {

        float DisFromTarget = Vector3.Magnitude(target - source);
        float CharaterSpeed = sSpeed;

        float TimeToTarget = DisFromTarget / CharaterSpeed;
        float TimeSize = TimeToTarget;

        float TargetSpeed = tSpeed;
        float targetMaxDis = TargetSpeed * TimeToTarget;
        float sourceMaxDis = sSpeed * TimeToTarget;

        Vector3 NewTargetPos = target + tDirection * targetMaxDis;
        Vector3 NewcharacterPos = source + Vector3.Normalize(NewTargetPos - source) * sourceMaxDis;

        float newDisFromTarget = Vector3.Magnitude(NewTargetPos - NewcharacterPos);
        for (int i = 0; i < 10; i++)
        {
            if (newDisFromTarget < DisFromTarget)
            {
                TimeSize = TimeSize / 2f;
                TimeToTarget += TimeSize;
            }
            else
            {
                TimeSize = TimeSize / 2f;
                TimeToTarget -= TimeSize;
            }

            TargetSpeed = tSpeed;
            targetMaxDis = TargetSpeed * TimeToTarget;
            sourceMaxDis = sSpeed * TimeToTarget;

            NewTargetPos = target + tDirection * targetMaxDis;
            NewcharacterPos = source + Vector3.Normalize(NewTargetPos - source) * sourceMaxDis;
            newDisFromTarget = Vector3.Magnitude(NewTargetPos - NewcharacterPos);
        }

        return NewTargetPos;
    }

    public static Vector3 CalculateInterceptionPoint(Vector3 source, Vector3 target, Vector3 tDirection, float sSpeed, float tSpeed)
    {
        Vector3 targetPosition = target;
        Vector3 targetVelocity = tDirection * tSpeed;
        Vector3 bulletPosition = source;
        float bulletSpeed = sSpeed;
        return CalculateInterceptionPoint(targetPosition, targetVelocity, bulletPosition, bulletSpeed);
    }
    public static Vector3 CalculateInterceptionPoint(Vector3 targetPosition, Vector3 targetVelocity, Vector3 bulletPosition, float bulletSpeed)
    {
        Vector3 relativePosition = targetPosition - bulletPosition;
        float targetSpeedSquared = targetVelocity.sqrMagnitude;
        float bulletSpeedSquared = bulletSpeed * bulletSpeed;

        // Quadratic coefficients
        float a = bulletSpeedSquared - targetSpeedSquared;
        float b = -2f * Vector3.Dot(relativePosition, targetVelocity);
        float c = relativePosition.sqrMagnitude;

        // Solve quadratic equation
        float discriminant = b * b - 4f * a * c;
        if (discriminant < 0)
        {
            // No solution, target is unreachable
            return Vector3.zero;
        }

        float sqrtDiscriminant = Mathf.Sqrt(discriminant);
        float t1 = (-b + sqrtDiscriminant) / (2f * a);
        float t2 = (-b - sqrtDiscriminant) / (2f * a);

        // Use the smallest positive time
        float interceptionTime = Mathf.Max(t1, t2);
        if (interceptionTime < 0)
        {
            // No valid interception time
            return Vector3.zero;
        }

        // Calculate interception point
        return targetPosition + targetVelocity * interceptionTime;
    }
}

public struct float_lerp
{
    private float start;
    private float end;
    private Timer timer;

    public void init(float start = 0.0f, float end = 0.0f)
    {
        this.start = start;
        this.end = end;
        timer.SetTimerTime(1.0f);
    }
    public void setStart(float start)
    {
        this.start = start; 
    }
    public void setEnd(float end)
    {
        this.end = end;
    }
    public void StartLerp()
    {
        timer.SetTimerTime(1.0f);
        timer.ActivateTimer();
    }
    public float Update(float amout)
    {
            timer.SubtractTimerByValue(amout);
            return Mathf.Lerp(start, end, timer.GetCurrentTime());
    }

}
