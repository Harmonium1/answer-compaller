namespace AnswerCompiler.States;

public interface IState
{
    Task OnEnter();
    Task<IState> Promote();
}