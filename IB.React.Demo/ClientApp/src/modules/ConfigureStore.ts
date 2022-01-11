import createSagaMiddleware from "redux-saga";
import {connectRouter, routerMiddleware} from "connected-react-router";
import {applyMiddleware, combineReducers, createStore} from "@reduxjs/toolkit";
import {composeWithDevTools} from "redux-devtools-extension";
import {IRootState, reducers, rootSaga} from "./index";
import {History} from "history";

/**
 * 스토어를 생성하여 반환합니다.
 * @param history History 객체
 * @param initialState 초기 상태
 */
export default function configureStore(history: History, initialState?: IRootState) {
    // 미들웨어 생성
    const sagaMiddleware = createSagaMiddleware();
    const middlewares = [
        // Connected Router 사용
        routerMiddleware(history),
        sagaMiddleware
    ];

    // 루트 리듀서 생성
    const rootReducer = combineReducers({
        ...reducers,
        router: connectRouter(history)
    });

    // Devtools 연결
    const enhancer = composeWithDevTools({trace: true, traceLimit: 25});

    // 스토어 생성
    const store = createStore(
        rootReducer,
        initialState,
        // dev tools와 기타 미들웨어들을 compose하여 사용합니다.
        enhancer(applyMiddleware(...middlewares))
    )
    sagaMiddleware.run(rootSaga)

    return store;
}