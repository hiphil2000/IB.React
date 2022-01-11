import React from "react";
import {Menu, MenuItem} from "@mui/material";
import {PopoverProps} from "@mui/material/Popover";

interface INavUserMenuProps {
    anchorEl: PopoverProps['anchorEl'];
    open: boolean,
    onClose: PopoverProps['onClose'];
    onLogout?: React.MouseEventHandler;
}

export default function NavUserMenu({
    anchorEl,
    open,
    onClose,
    onLogout
}: INavUserMenuProps) {
    
    return (
        <Menu id="nav-user-menu"
              anchorEl={anchorEl}
              open={open} onClose={onClose}
        >
            <MenuItem onClick={onLogout}>로그아웃</MenuItem>
        </Menu>
    )
}