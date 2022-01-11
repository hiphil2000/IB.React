import React, {useEffect, useState} from 'react';
import {BrowserRouter} from 'react-router-dom';
import PageRoute from "./libs/PageRoute";
import {ThemeProvider} from "@mui/material";
import theme from "./libs/mui/Theme";
import useAuth from "./libs/hooks/UseAuth";

function App() {
    const {currentUser} = useAuth();
    // const authStore = useSelector((state: IRootState) => state.auth);
    // const userStore = useSelector((state: IRootState) => state.auth.getCurrentUser);
    //
    // const [isLoading, setLoading]= useState<boolean>(true);
    //
    // useEffect(() => {
    //     setLoading(authStore.user === null && userStore.data === null && userStore.error === null);
    //     console.log(authStore.user === null && userStore.data === null && userStore.error === null);
    // }, [authStore])
    
    return (
        <ThemeProvider theme={theme}>
            <BrowserRouter>
                <PageRoute currentUser={currentUser} />
            </BrowserRouter>
        </ThemeProvider>
    );
}

export default App;
