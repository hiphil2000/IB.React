import React, {useEffect} from "react";
import useCommonCode from "../../libs/hooks/UseCommonCode";
import {ICommonCode} from "../../libs/apis/Core";

interface IHomeContainerProps {
    
}

const loading = "%%LOADING%%";

export default function HomeContainer({
    
}: IHomeContainerProps) {
    const t1 = useCommonCode("TEST_0001");
    const t2 = useCommonCode("TEST_0002");
    
    const render = (datas: ICommonCode[]) => {
        return (
            datas.map(data => (
                <option key={data.codeId} value={data.codeId}>{data.codeName}</option>
            ))
        )
    }
    
    return (
        <div>
            <h3>Home</h3>
            <select value={t1.loading ? loading : t1.value} 
                    onChange={t1.onChange}
            >
                {
                    t1.loading 
                        ? <option disabled id="%%LOADING%%">Loading...</option> 
                        : ""
                }
                {t1.codes !== null && render(t1.codes || [])}
            </select>
        </div>
    )
}