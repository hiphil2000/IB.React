import React, {useEffect, useState} from "react";
import {Button, styled, TextField, Typography} from "@mui/material";
import {ILoginPayload} from "../../libs/apis/Auth";
import {useHistory} from "react-router";
import useAuth from "../../libs/hooks/UseAuth";

export interface ILoginFormProps {
    
}

export default function LoginForm({
    
}: ILoginFormProps) {
    const history = useHistory();
    const {currentUser, login, loginState} = useAuth();

    // 폼 임력 State
    const [formState, setFormState] = useState<ILoginPayload>({
        userId: "",
        password: ""
    });
    
    // 폼 입력 이벤트 핸들러
    const handleInput = (e: React.FormEvent<HTMLInputElement>) => {
        const target = e.target as HTMLInputElement; 
        const id = target.id;
        const value = target.value;
        
        // 유효한 입력이면 State를 업데이트합니다.
        if (Object.keys(formState).includes(id)) {
            setFormState(prev => ({
                ...prev,
                [id]: value
            }))
        }
    }
    
    // 로그인 이벤트 핸들러
    const handleLogin = () => {
        login(formState);
    }
    
    // CurrentUser가 있다면, 홈으로 보냅니다.
    useEffect(() => {
        if (currentUser) {
            history.push("/");
        }
    }, [currentUser]);
    
    return (
        <FormContainer>
            <Typography variant="h5">Login</Typography>
            <TextField id="userId" 
                       value={formState.userId}
                       onInput={handleInput}
            />
            <TextField id="password" type="password" 
                       value={formState.password}
                       onInput={handleInput}
            />
            <Button id="loginButton" onClick={handleLogin} disabled={loginState.loading}>Login</Button>
        </FormContainer>
    )
}

const FormContainer = styled("form")({
    display: "flex",
    flexDirection: "column",
    flex: 1,
    margin: "1em",
    "& > *": {
        flex: 1
    }
});