namespace PreCommitHooks.Checks
{
    public interface IPreCommitCheck
    {
        bool CanCommit();
    }
}