import {all} from "redux-saga/effects";
import {authReducer, AuthState, currentUserSaga} from "./auth";

// 모든 리듀서의 목록입니다.
export const reducers = {
    auth: authReducer
}

// 스토어의 RootState 인터페이스입니다.
export interface IRootState {
    auth: AuthState
}

export function* rootSaga() {
    yield all([
        currentUserSaga()
    ]);
}