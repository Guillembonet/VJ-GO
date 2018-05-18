using UnityEngine;

public class Utility {

    public static float climbOffset = 0.25f;

    public static Vector3 Vector3Round(Vector3 inputVector)
    {
        return new Vector3(Mathf.Round(inputVector.x), Mathf.Round(inputVector.y), Mathf.Round(inputVector.z));
    }

    public static Vector2 Vector2Round(Vector2 inputVector)
    {
        return new Vector2(Mathf.Round(inputVector.x), Mathf.Round(inputVector.y));
    }
    public static bool AreDiagonallyAligned(Vector3 start, Vector3 end)
    {
        if (start.y != end.y && (start.x != end.x || start.z != end.z)) return true;
        else return false;
    }

    public static bool AreParallelToFloor(Vector3 start, Vector3 end)
    {
        if (start.y - end.y == 0f) return true;
        else return false;
    }

    public static bool AreVerticallyAligned(Vector3 start, Vector3 end)
    {
        if (start.y != end.y && start.x == end.x && start.z == end.z) return true;
        else return false;
    }

    public static bool Arg1IsHigherThanArg2(Vector3 arg1, Vector3 arg2)
    {
        if (arg1.y - arg2.y > 0f) return true;
        else return false;
    }

    public static void GetClimbOffset(ref float x, ref float z, Vector3 destPos, Vector3 forward)
    {
        if (Mathf.Round(forward.x) == -1f)
        {
            x = destPos.x + Utility.climbOffset;
            z = destPos.z;
        }
        else
        {
            x = destPos.x;
            z = destPos.z + Utility.climbOffset;
        }
    }

    public static void GetClimbDescendOffset(ref float x, ref float z, Vector3 destPos, Vector3 forward)
    {
        if (Mathf.Round(forward.x) == 1.0f)
        {
            x = destPos.x + Utility.climbOffset;
            z = destPos.z;
        }
        else
        {
            x = destPos.x;
            z = destPos.z + Utility.climbOffset;
        }
    }
}
