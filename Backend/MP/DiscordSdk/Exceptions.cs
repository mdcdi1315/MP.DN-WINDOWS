

namespace MP.DGSDK
{
    public class ResultException : ExceptionSystem.BaseException
    {
        private Result m_result;

        public ResultException(Result result) : base(result.ToString())
        {
            m_result = result;
        }

        public Result Result => m_result;
    }

}