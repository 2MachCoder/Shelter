using System;

namespace CodeBase.Services.App
{
    public interface IApplicationService
    {
        event Action ApplicationPause;
        event Action ApplicationResume;
        event Action ApplicationFocus;
        event Action ApplicationFocusLost;
    }
}