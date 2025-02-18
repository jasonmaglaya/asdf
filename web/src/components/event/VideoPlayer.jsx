import styled from "styled-components";

const IFrameWrapper = styled.div`
  position: relative;
  width: 100%;
  height: 0;
  padding-bottom: 56.25%;

  iframe {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
  }
`;

export default function VideoPlayer({ embedTag }) {
  return (
    <IFrameWrapper
      className="embed-responsive embed-responsive-16by9"
      dangerouslySetInnerHTML={{ __html: embedTag }}
    ></IFrameWrapper>
  );
}
