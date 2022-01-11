import React from "react";
import {Container, ContainerProps} from "@mui/material";
import NavBar from "./NavBar";

export interface IPageTemplateProps {
    children?: React.ReactNode;
    maxWidth?: ContainerProps["maxWidth"];
}

export default function PageTemplate({
    children,
    maxWidth
}: IPageTemplateProps) {
    return (
        <>
            <NavBar />
            <Container maxWidth={maxWidth || "lg"}>
                {children}
            </Container>
        </>
    )
}