import React from "react";
import {Link as RouterLink, LinkProps as RouterLinkProps} from "react-router-dom";
import Link, {LinkProps} from "@mui/material/Link";

/**
 * MUI의 링크를 react-router-dom에 연결하기 위한 Behavior입니다.
 */
const linkBehavior = React.forwardRef<
    any, 
    Omit<RouterLinkProps, 'to'> & { href: RouterLinkProps["to"] }
>((props, ref) => {
    const {href, ...others} = props;
    
    return (
        <RouterLink ref={ref} to={href} {...others} />
    )
});

export default linkBehavior;