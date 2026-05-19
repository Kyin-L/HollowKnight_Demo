public class Breakable_Save : Breakable
{
    protected override void OnBreak(DamageInfo info)
    {
        base.OnBreak(info);


        //修改保存数据

        //
        Destroy(gameObject);
    }
}
