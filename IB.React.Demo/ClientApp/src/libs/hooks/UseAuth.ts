import {useDispatch, useSelector} from "react-redux";
import useFetch from "./UseFetch";
import {IUser} from "../apis/Interfaces";
import {ILoginPayload, LoginResponse, Login, Logout} from "../apis/Auth";
import {useCallback, useEffect, useState} from "react";
import {StorageHelper} from "../local-storage";
import {setUser} from "../../modules/auth";
import {userSelector} from "../../modules/auth/Seletors";
import {USER_KEY} from "../local-storage/StorageHelper";

/**
 * 인증과 관련된 기능을 갖고 있는 Hook입니다.
 * @return {{currentUser: IUser, logout: () => void, loginState: IAsyncState<LoginResponse>, login: (payload: ILoginPayload) => void}}
 */
export default function useAuth() {
    const dispatch = useDispatch();
    
    // 사용할 스토어들
    const userStore = useSelector(userSelector);
    
    // 요청 Utils
    const storage = StorageHelper.getInstance();
    const [logoutState, logout] = useFetch<void, void>(Logout);
    const [loginState, login] = useFetch<ILoginPayload, LoginResponse>(Login);
    
    // Saga 요청 핸들링을 위한 State
    const [isLoginRequest, setLoginRequest] = useState<boolean>(false);
    
    // 유저 설정 함수입니다.
    const applyUser = (user: IUser | null): void => {
        // 스토리지 설정
        storage.set(USER_KEY, user);
        
        console.log(user);
        
        // 스토어 설정
        dispatch(setUser(user));
    }
    
    // 로그인 함수입니다.
    const handleLogin = useCallback((payload: ILoginPayload) => {
        // 이미 로그인 요청을 했다면, 무시합니다.
        if (isLoginRequest) {
            return;
        }
        
        // 로그인 요청을 진행합니다.
        setLoginRequest(true);
        login(payload);
    }, []);
    
    // 로그인 State 처리용 Effect입니다.
    useEffect(() => {
        if (loginState.loading === false && loginState.data?.data !== null && loginState.data?.data.user !== null) {
            if (isLoginRequest) {
                setLoginRequest(false);
                applyUser(loginState.data?.data.user!);
            }
        }
    }, [isLoginRequest, loginState])
    
    // 로그아웃 함수입니다.
    const handleLogout = useCallback(() => {
        logout();
        applyUser(null);
    }, []);
    
    return {
        currentUser: userStore,
        login: handleLogin,
        loginState: loginState,
        logout: handleLogout
    }
}