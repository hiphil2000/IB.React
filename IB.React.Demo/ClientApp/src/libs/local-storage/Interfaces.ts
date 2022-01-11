/**
 * LocalStorageHelper 클래스의 기본 인터페이스입니다.
 */
export interface IStorageHelper<TData> {
    /**
     * 데이터를 조회합니다.
     * 존재하지 않는다면 null을 반환합니다.
     * @return {IUser | null} 스토리지에 저장된 데이터
     */
    get: () => TData | null;

    /**
     * 데이터를 저장합니다.
     * 매개변수가 null이라면 데이터를 삭제합니다.
     * @param {TData | null} data 스토리지에 저장할 데이터
     */
    set: (data: TData | null) => void;

    /**
     * 데이터의 존재 여부를 확인합니다.
     * @return {boolean} 데이터 존재 여부
     */
    exists: () => boolean;
}