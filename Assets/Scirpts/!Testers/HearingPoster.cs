using UnityEngine;

public static class HearingPoster
{
    public static LayerMask layer = LayerMask.GetMask("Enemy");
    public static Collider2D[] colliders;

    public static void PostVoice(Vector3 pos,float radius)
    {
        if (colliders == null)
        {
            colliders = new Collider2D[16];
        }
        int x = Physics2D.OverlapCircleNonAlloc(pos,radius,colliders,layer);
        if(x != 0)
        {
            for (int i = 0; i < x; i++) {
                colliders[i].GetComponent<IHearingReceiver>().OnHeard(pos);
            }
        }
    }
}
