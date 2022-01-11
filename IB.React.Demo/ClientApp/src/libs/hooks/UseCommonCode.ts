import useFetch from "./UseFetch";
import {GetCommonCodeResponse, GetCommonCodes, ICommonCode} from "../apis/Core";
import React, {useEffect, useState} from "react";
import {useSelector} from "react-redux";
import {ICommonResponse} from "../apis/Interfaces";

export default function useCommonCode(groupId: string) {
    const [resultList, fetch] = useFetch<string, GetCommonCodeResponse>(GetCommonCodes);
    const [value, setValue] = useState<string>("");
    
    useEffect(() => {
        fetch(groupId);
    }, [fetch, groupId]);
    
    const onChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const target = e.target as HTMLSelectElement;
        setValue(target.value);
    }
    
    return {
        codes: resultList.data?.data,
        loading: resultList.loading,
        error: resultList.error,
        message: resultList.data?.message,
        value,
        onChange
    };
}