import { Carousel } from "react-bootstrap";

export default function EventCarousel({ events }) {
  return (
    <div className="p-5 m-1 bg-dark text-white rounded-3">
      {!events?.length ? (
        <div className="text-center">
          <span className="h1">COMING SOON</span>
        </div>
      ) : (
        <Carousel>
          {events?.map((event) => {
            return (
              <Carousel.Item key={event.id}>
                <div className="text-center py-5">
                  <h1 className="display-5 fw-bold">{event.title}</h1>
                  <p className="fs-4">{event.subTitle}</p>
                  <div>
                    <a
                      className="btn btn-primary btn-lg align-center"
                      type="button"
                      href={`/events/${event.id}`}
                    >
                      <h3>ENTER</h3>
                    </a>
                  </div>
                </div>
              </Carousel.Item>
            );
          })}
        </Carousel>
      )}
    </div>
  );
}
