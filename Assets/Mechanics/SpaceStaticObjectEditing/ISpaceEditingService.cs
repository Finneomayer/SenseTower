namespace Mechanics.SpaceStaticObjectEditing
{
    public interface ISpaceEditingService
    {
        public void ToogleEditingMode();

        public bool EditingModeIsEnable();
        public void InvokeSaveOnClient();
        public void SaveData();
    }
}