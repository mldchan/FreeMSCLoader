using System;

namespace MSCLoader;

internal class ScrollRectCulling : MonoBehaviour
{
    //Custom ScrollRect culling (invisible text still takes shitton of verts)

    public RectTransform rt;
    public GameObject details;
    private readonly Vector3[] v = new Vector3[4];

    public void FixedUpdate()
    {
        rt.GetWorldCorners(v);

        var maxY = Math.Max(v[0].y, v[1].y);
        var minY = Math.Min(v[0].y, v[1].y);

        if (maxY < 0 || minY > Screen.height)
            details.SetActive(false);
        else
            details.SetActive(true);
    }
}