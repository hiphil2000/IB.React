import {createReducer} from "typesafe-actions";
import {AuthAction, AuthState} from "./Types";
import {getCurrentUserAsync, SET_USER} from "./Actions";
import {asyncStateHelper, createAsyncReducer} from "../ReducerUtils";

const initialState: AuthState = {
    user: null,
    getCurrentUser: asyncStateHelper.initial(),
}

/**
 * 현재 사용자 조회 액션의 리듀서 핸들러입니다.
 * @type {(state: AuthState, action: AnyAction) => AuthState}
 */
const getCurrentUserHandler = createAsyncReducer<AuthState, typeof getCurrentUserAsync, keyof AuthState>(getCurrentUserAsync, "getCurrentUser");

const authReducer = createReducer<AuthState, AuthAction>(initialState, {
    [SET_USER]: (state, action) => ({
        ...state,
        user: action.payload
    }),
    ...getCurrentUserHandler,
})

export default authReducer;