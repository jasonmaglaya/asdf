import { Modal } from "react-bootstrap";
import UsersList from "./UsersList";
import { useState } from "react";
import { searchUser } from "../../services/usersService";
import { useSelector } from "react-redux";

export default function SearchUserDialog({
  show,
  handleClose,
  handleUserClick,
  isAgent,
  includeSelf,
  currency,
  locale,
}) {
  const pageSize = 30;
  const [users, setUsers] = useState([]);
  const [isSearching, setIsSearching] = useState();
  const { user } = useSelector((state) => state.user);

  const handleSearchTextChange = (keyword) => {
    if (!keyword?.length) {
      setUsers([]);
      return;
    }

    //setIsSearching(true);

    searchUser(keyword, 1, pageSize, isAgent)
      .then(({ data }) => {
        if (includeSelf) {
          setUsers(data?.result?.list);
        } else {
          setUsers(data?.result?.list?.filter((x) => x.id !== user?.id));
        }
      })
      .finally(() => setIsSearching(false));
  };

  return (
    <Modal show={show} centered animation={false} onHide={handleClose}>
      <Modal.Header closeButton>
        <Modal.Title>Search User</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <UsersList
          users={users}
          handleUserClick={handleUserClick}
          handleSearchTextChange={handleSearchTextChange}
          isSearching={isSearching}
          currency={currency}
          locale={locale}
        />
      </Modal.Body>
    </Modal>
  );
}
