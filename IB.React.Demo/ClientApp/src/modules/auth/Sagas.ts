import {GET_CURRENT_USER, getCurrentUserAsync} from "./Actions";
import {GetCurrentUser} from "../../libs/apis/Auth";
import {call, put, takeLatest} from "redux-saga/effects";
import {AxiosError} from "axios";
import {IUser} from "../../libs/apis/Interfaces";

/**
 * GetCurrentUser Saga입니다.
 */
function* _GetCurrentUserSaga() {
    try {
        const result: IUser = yield call(GetCurrentUser);
        yield put(getCurrentUserAsync.success(result))
    } catch (e) {
        yield put(getCurrentUserAsync.failure(e as AxiosError))
    }
}

/**
 * GetCurrentUser Wrapper Saga입니다.
 */
export function* currentUserSaga() {
    yield takeLatest(GET_CURRENT_USER, _GetCurrentUserSaga); 
}