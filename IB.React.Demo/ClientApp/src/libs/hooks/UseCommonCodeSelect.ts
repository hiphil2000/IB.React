import React, {ChangeEvent} from "react";

/**
 * CommonCodeSelect 컨트롤의 필요한 요소를 반환하는 Hook입니다.
 * @param groupId
 * @param initialValue
 */
export default function useCommonCodeSelect(groupId: string, initialValue: string) {
    const [value, setValue] = React.useState<string>(initialValue);
    const onChange = (event: ChangeEvent) => {
        const target = event.target as HTMLSelectElement;
        
        setValue(target.value);
    }
    
    return {
        groupId,
        value, 
        setValue, 
        onChange
    }
}