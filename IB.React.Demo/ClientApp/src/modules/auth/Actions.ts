import {deprecated} from "typesafe-actions";
import {createAsyncAction} from "typesafe-actions";
import {IUser} from "../../libs/apis/Interfaces";
import {AxiosError} from "axios";
import {ILoginPayload, LoginResponse} from "../../libs/apis/Auth";

const {
    createStandardAction
} = deprecated;

export const SET_USER = "auth/SET_USER";

export const GET_CURRENT_USER = "auth/GET_CURRENT_USER";
export const GET_CURRENT_USER_SUCCESS = "auth/GET_CURRENT_USER_SUCCESS";
export const GET_CURRENT_USER_FAILURE = "auth/GET_CURRENT_USER_FAILURE";

export const REQUEST_LOGIN = "auth/REQUEST_LOGIN";
export const REQUEST_LOGIN_SUCCESS = "auth/REQUEST_LOGIN_SUCCESS";
export const REQUEST_LOGIN_FAILURE = "auth/REQUEST_LOGIN_FAILURE";

// 사용자 지정 액션 생성자입니다.
export const setUser = createStandardAction(SET_USER)<IUser | null>();

// 현재 사용자 조회 액션 생성자입니다.
export const getCurrentUserAsync = createAsyncAction(
    GET_CURRENT_USER,
    GET_CURRENT_USER_SUCCESS,
    GET_CURRENT_USER_FAILURE
)<void, IUser, AxiosError>();