import {useCallback, useReducer, useState, ReducerAction, Reducer} from "react";
import {AxiosError} from "axios";

export interface IAsyncState<TResult> {
    loading: boolean;
    data: TResult | null;
    error: AxiosError | null;
}

const LOADING: string = "ASYNC_LOADING";
const SUCCESS: string = "ASYNC_SUCCESS";
const ERROR: string = "ASYNC_ERROR";

export interface IAction<TPayload> {
    type: string,
    payload?: TPayload | AxiosError
}

const initializeState: IAsyncState<any> = {
    loading: false,
    data: null,
    error: null
}

/**
 * 
 * @param api
 */
export default function useFetch<TPayload, TResult>(
    api: (payload: TPayload) => Promise<TResult>
): [IAsyncState<TResult>, (payload: TPayload) => Promise<void>] {
    // 동적으로 리듀서를 생성합니다.
    type reducerType = (state: IAsyncState<TResult>, action: IAction<TResult>) => IAsyncState<TResult>;
    const reducer = useCallback<reducerType>((state, action) => {
        switch(action.type) {
            case LOADING:
                return {
                    ...state,
                    loading: true
                }
            case SUCCESS:
                return {
                    ...state,
                    loading: false,
                    data: action.payload as TResult
                }
            case ERROR:
                return {
                    ...state,
                    loading: false,
                    error: action.payload as AxiosError
                }
            default:
                return state;
        }
    }, [api]);

    // useReducer 사용
    const [state, dispatch] = useReducer(reducer, initializeState);

    // Api를 호출할 수 있는 함수를 생성합니다.
    const fetchData = useCallback(async (payload: TPayload) => {
        dispatch({type: LOADING});
        try {
            const response = await api(payload);
            dispatch({type: SUCCESS, payload: response});
        } catch (e) {
            dispatch({type: ERROR, payload: e as AxiosError});
        }
    }, [api]);
    
    // 결과를 반환합니다.
    return [state, fetchData];
}