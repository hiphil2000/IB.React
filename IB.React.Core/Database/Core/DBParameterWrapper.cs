using System.Data;
using System.Data.Common;

namespace IB.React.Core.Database.Core
{
    public class DbParameterWrapper
    {
        /// <summary>
        /// 매개변수 명
        /// </summary>
        public string ParameterName;

        /// <summary>
        /// DbType
        /// </summary>
        public DbType DbType;

        /// <summary>
        /// 매개변수 값
        /// </summary>
        public object Value;

        /// <summary>
        /// 매개변수 크기
        /// </summary>
        public int Size;

        /// <summary>
        /// 입출력 매개변수 형식
        /// </summary>
        public ParameterDirection Direction;

        /// <summary>
        /// 매개변수가 IN 구문에 사용되는지 여부
        /// </summary>
        public bool InClause;

        /// <summary>
        /// <para>매개변수 Wrapper 클래스의 새 인스턴스를 초기화합니다.</para>
        /// </summary>
        /// <param name="parameter">명령작업에서 생성된 DbParameter 인스턴스</param>
        /// <returns>Wrapper 클래스의 멤버 변수들이 가지고있는 값으로 생성된 DbParameter</returns>
        public DbParameter GetParameter(DbParameter parameter)
        {
            parameter.ParameterName = ParameterName;
            parameter.DbType = DbType;
            parameter.Value = Value;
            parameter.Direction = Direction;

            if (Size != -1)
                parameter.Size = Size;

            return parameter;
        }
    }
}