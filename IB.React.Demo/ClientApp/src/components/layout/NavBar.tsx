import React, {useEffect} from "react";
import {AppBar, Box, Button, Container, Toolbar, Typography} from "@mui/material";
import Axios from "axios";
import pageList from "../../pages/PageList";
import {useHistory} from "react-router";
import useAuth from "../../libs/hooks/UseAuth";
import {LoginInfo} from "../../pages/auth";
import NavUser from "./NavUser";

interface INavBarProps {
}

export default function NavBar({
}: INavBarProps) {
    const auth = useAuth();
    const history = useHistory();
    const handleNavigation = (e: React.MouseEvent<HTMLButtonElement>) => {
        const target = e.target as HTMLButtonElement;
        const pageInfo = pageList.find(page => page.pageId === target.id)
        
        if (pageInfo) {
            history.push(pageInfo.path);
        }
    }
    
    return (
        <AppBar position="static">
            <Container maxWidth="lg">
                <Toolbar disableGutters={true} sx={{
                    "& > *": {
                        color: "white"
                    }
                }}>
                    <Box sx={{
                        flexGrow: 1
                    }}>
                        {pageList.map(page => {
                            if (auth.currentUser !== null && page.pageId === LoginInfo.pageId) {
                                return;
                            }
                            return (
                                <Button id={page.pageId} key={page.pageId}
                                        onClick={handleNavigation}
                                        sx={{
                                            color: "white",
                                            mr: "0.25em",
                                            "&:nth-last-of-type(1)": {
                                                mr: 0
                                            }
                                        }}
                                >
                                    {page.pageName}
                                </Button>
                            )
                        })}
                    </Box>
                    <NavUser user={auth.currentUser} onLogout={auth.logout} />
                </Toolbar>
            </Container>
        </AppBar>
    );
}