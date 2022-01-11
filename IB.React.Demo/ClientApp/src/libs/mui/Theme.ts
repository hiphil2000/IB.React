import {createTheme} from "@mui/material";
import {LinkProps} from "@mui/material/Link";
import LinkBehavior from "./LinkBehavior";

const theme = createTheme({
    components: {
        // 기본 값을 react-router-dom으로 연결합니다.
        MuiLink: {
            defaultProps: {
                component: LinkBehavior
            } as LinkProps
        },
        MuiList: {
            defaultProps: {
                dense: true
            },
        },
        MuiMenuItem: {
            defaultProps: {
                dense: true
            },
        },
        MuiTable: {
            defaultProps: {
                size: 'small',
            }
        },
        MuiButton: {
            defaultProps: {
                size: 'small',
            }
        },
        MuiButtonGroup: {
            defaultProps: {
                size: 'small',
            }
        },
        MuiCheckbox: {
            defaultProps: {
                size: 'small',
            }
        },
        MuiFab: {
            defaultProps: {
                size: 'small',
            }
        },
        MuiFormControl: {
            defaultProps: {
                margin: 'dense',
                size: 'small',
            }
        },
        MuiFormHelperText: {
            defaultProps: {
                margin: 'dense',
            }
        },
        MuiIconButton: {
            defaultProps: {
                size: 'small',
            }
        },
        MuiInputBase: {
            defaultProps: {
                margin: 'dense',
            }
        },
        MuiInputLabel: {
            defaultProps: {
                margin: 'dense',
            }
        },
        MuiRadio: {
            defaultProps: {
                size: 'small',
            }
        },
        MuiSwitch: {
            defaultProps: {
                size: 'small',
            }
        },
        MuiTextField: {
            defaultProps: {
                margin: 'dense',
                size: 'small',
            }
        },
    },
    palette: {
        primary: {
            main: '#3f51b5',
        },
        secondary: {
            main: '#f50057',
        },
    },
    typography: {
        fontSize: 12,
    },
    spacing: 4,
});

export default theme;