import React, {useCallback, useState} from "react";

/**
 * 단순한 Boolean Toggle State를 처리하기 위한 Hook입니다.
 * @param {boolean} initialState 초기 상태
 * @return {{setState: (value: (((prevState: boolean) => boolean) | boolean)) => void, toggle: () => void, state: boolean}}
 */
export default function useToggle(initialState?: boolean) {
    const [state, setState] = useState(initialState || false);
    const toggle = useCallback(() => {
        setState(!state);
    }, [])
    
    return {state, toggle, setState};
}