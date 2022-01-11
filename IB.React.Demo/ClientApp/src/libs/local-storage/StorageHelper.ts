/**
 * LocalStorage 탐색을 위한 Helper 함수입니다.
 */
export default class StorageHelper {
    private static instance: StorageHelper | null = null;
    
    private constructor() {
        
    }
    
    public static getInstance(): StorageHelper {
        if (StorageHelper.instance === null) {
            StorageHelper.instance = new StorageHelper();
        }
        
        return StorageHelper.instance;
    }

    /**
     * LocalStorage 데이터를 조회합니다.
     * 만약 데이터가 없다면 null을 반환합니다.
     * @param {string} key LocalStorage 데이터 키
     * @return {T | null} LocalStorage 데이터
     */
    public get<T>(key: string): T | null {
        const data = localStorage.getItem(key);

        return data ?
            JSON.parse(data) as T :
            null;
    }

    /**
     * LocalStorage에 데이터를 저장합니다.
     * 만약 null이라면 데이터를 삭제합니다.
     * @param {string} key 저장할 데이터 키
     * @param {T | null} data 저장할 데이터
     */
    public set<T>(key: string, data: T | null): void {
        if (data != null) {
            localStorage.setItem(key, JSON.stringify(data));
        } else {
            localStorage.removeItem(key);
        }
    }
    
    /**
     * 해당 데이터 존재 여부를 확인합니다.
     * @param {string} key 데이터 키
     * @return {boolean} 데이터 존재 여부입니다.
     */
    public exists(key: string): boolean {
        return this.get(key) !== null;
    }
}

export const USER_KEY = 'UserData';