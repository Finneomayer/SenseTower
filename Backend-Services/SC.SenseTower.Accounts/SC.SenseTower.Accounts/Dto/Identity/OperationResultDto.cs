namespace SC.SenseTower.Accounts.Dto.Identity
{
    public class OperationResultDto
    {
        public bool Succeeded { get; set; }

        public string Message { get; set; } = null!;
    }
}
