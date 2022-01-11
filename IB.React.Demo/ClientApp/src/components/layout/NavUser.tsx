import React, {useCallback, useState} from "react";
import {IUser} from "../../libs/apis/Interfaces";
import {Avatar, Button} from "@mui/material";
import {LoginInfo} from "../../pages/auth";
import {useHistory} from "react-router";
import useToggle from "../../libs/hooks/UseToggle";
import NavUserMenu from "./NavUserMenu";

interface INavUserProps {
    user: IUser | null,
    onLogout: () => void;
}

export default function NavUser({
    user,
    onLogout
}: INavUserProps) {
    const history = useHistory();
    const {state: open, toggle, setState: setOpen} = useToggle(false);
    const [anchorEl, setAnchorEl] = useState<Element | undefined>(undefined);
    
    const gotoLogin = useCallback(() => {
        history.push(LoginInfo.path);
    }, [LoginInfo])
    
    const handleAvatarClick = (e: React.MouseEvent) => {
        setAnchorEl(e.target as Element);
        toggle();
    }
    
    const handleMenuClose = () => {
        setOpen(false);
    }
    
    return (
        <>
        {
            user ?
            <>
                <Button onClick={handleAvatarClick}>
                    <Avatar>{user.userName}</Avatar>
                </Button>
                <NavUserMenu anchorEl={anchorEl} open={open} 
                             onClose={handleMenuClose} 
                             onLogout={onLogout} />
            </> :
            <Button id={LoginInfo.pageId} key={LoginInfo.pageId}
                    onClick={gotoLogin}
                    sx={{
                        color: "white",
                        mr: "0.25em",
                        "&:nth-last-of-type(1)": {
                            mr: 0
                        }
                    }}
            >
                {LoginInfo.pageName}
            </Button>
                
        }
        </>
    )
}