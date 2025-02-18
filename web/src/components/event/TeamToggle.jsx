import styled from "styled-components";

export const TeamToggle = styled.div.attrs((props) => ({
  className: props.className,
}))`
  label {
    color: ${(props) => props.color};
    border-color: ${(props) => props.color} !important;
  }

  .btn-check:hover + .btn {
    color: ${(props) => props.color};
    border-color: ${(props) => props.color} !important;
  }

  .btn-check:checked + .btn {
    background-color: ${(props) => props.color};
    color: white;
  }
`;
