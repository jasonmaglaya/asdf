import React from "react";

const LoadingScreen = () => {
  return (
    <div style={{ height: "80vh" }}>
      <div className="row h-100 justify-content-center align-items-center">
        <div className="col-12 text-center">
          <img src="/rooster.gif" alt="Loading..." style={{ height: "10em" }} />
          <h3 className="display-6 text-light">Loading...</h3>
        </div>
      </div>
    </div>
  );
};

export default LoadingScreen;
