import {Store} from "@reduxjs/toolkit";
import {IUser} from "./apis/Interfaces";
import {GetCurrentUser} from "./apis/Auth";
import {setUser} from "../modules/auth";

/**
 * 최초 실행 시 사용자를 불러오기 위한 함수입니다.
 * @param {Store} store Redux Store
 * @return {Promise<void>}
 */
export default async function loadUser(store: Store): Promise<void> {
    let user: IUser | null = null;

    // 서버에서 현재 유저를 조회합니다.
    try {
        user = (await GetCurrentUser()).data;
    } catch(e) {
        user = null;
    }
    
    // 조회한 사용자를 스토어에 넣습니다.
    store.dispatch(setUser(user));
}